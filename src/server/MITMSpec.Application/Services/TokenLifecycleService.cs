using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Configuration;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Enrollment;
using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Services;

public sealed class TokenLifecycleService(
    ITokenStore tokenStore,
    IPeerStore peerStore,
    IPeerAddressAllocator peerAddressAllocator,
    IAuditEntryStore auditEntryStore,
    IOptions<ProvisioningOptions> provisioningOptions) : ITokenLifecycleService
{
    public async Task<IssuedTokenDto> CreateAsync(CreateTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var token = new TokenDto(
            CreateTokenId(),
            request.UserId,
            TokenStatus.Pending,
            request.Description,
            now,
            now.AddHours(Math.Clamp(request.LifetimeHours, 1, 168)),
            null,
            null,
            null);
        var redeemSecret = CreateRedeemSecret();
        var saved = await tokenStore.CreateAsync(token, ComputeSecretHash(redeemSecret), cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "token.created",
                "token",
                saved.TokenId,
                request.ActorId,
                "success",
                $"Token was created for user '{saved.UserId}'."),
            cancellationToken);

        return new IssuedTokenDto(saved, redeemSecret);
    }

    public async Task<TokenRedeemResultDto?> RedeemAsync(string tokenId, RedeemTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await tokenStore.GetByIdAsync(tokenId, cancellationToken);
        if (current is null)
        {
            return null;
        }

        var evaluation = EvaluateRedeemability(current);
        if (evaluation is not null)
        {
            await auditEntryStore.AddAsync(
                new AuditEntryDto(
                    Guid.NewGuid().ToString("n"),
                    DateTimeOffset.UtcNow,
                    "token.redeem_failed",
                    "token",
                    current.TokenId,
                    request.ActorId,
                    "failure",
                    evaluation),
                cancellationToken);

            return null;
        }

        if (string.IsNullOrWhiteSpace(request.ClientPublicKey))
        {
            await auditEntryStore.AddAsync(
                new AuditEntryDto(
                    Guid.NewGuid().ToString("n"),
                    DateTimeOffset.UtcNow,
                    "token.redeem_failed",
                    "token",
                    current.TokenId,
                    request.ActorId,
                    "failure",
                    $"Token redemption failed for peer '{request.PeerId}' because no client public key was supplied."),
                cancellationToken);

            return null;
        }

        var storedSecretHash = await tokenStore.GetSecretHashAsync(tokenId, cancellationToken);
        if (storedSecretHash is null || !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(storedSecretHash),
                Encoding.UTF8.GetBytes(ComputeSecretHash(request.RedeemSecret))))
        {
            await auditEntryStore.AddAsync(
                new AuditEntryDto(
                    Guid.NewGuid().ToString("n"),
                    DateTimeOffset.UtcNow,
                    "token.redeem_failed",
                    "token",
                    current.TokenId,
                    request.ActorId,
                    "failure",
                    $"Token redemption failed for peer '{request.PeerId}' due to an invalid secret."),
                cancellationToken);

            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var updated = await tokenStore.UpsertAsync(current with { Status = TokenStatus.Redeemed, RedeemedAtUtc = now }, cancellationToken);
        var tunnelAddress = await peerAddressAllocator.AllocateAsync(cancellationToken);
        var peer = await peerStore.UpsertAsync(
            new PeerDto(
                request.PeerId,
                updated.UserId,
                updated.TokenId,
                tunnelAddress,
                request.ClientPublicKey,
                true,
                now,
                null),
            cancellationToken);
        var wireGuard = new WireGuardPeerConfigurationDto(
            tunnelAddress,
            provisioningOptions.Value.ClientDnsServer,
            provisioningOptions.Value.GatewayVpnEndpoint,
            provisioningOptions.Value.GatewayVpnPublicKey,
            provisioningOptions.Value.AllowedIps,
            provisioningOptions.Value.PersistentKeepaliveSeconds);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "token.redeemed",
                "token",
                updated.TokenId,
                request.ActorId,
                "success",
                $"Token was redeemed for user '{updated.UserId}'."),
            cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "peer.bound",
                "peer",
                peer.PeerId,
                request.ActorId,
                "success",
                $"Peer '{peer.PeerId}' was bound to user '{peer.UserId}' via token '{updated.TokenId}' with tunnel address '{peer.TunnelAddressCidr}'."),
            cancellationToken);

        return new TokenRedeemResultDto(updated, peer, wireGuard);
    }

    public async Task<TokenDto?> RevokeAsync(string tokenId, TokenActionRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await tokenStore.GetByIdAsync(tokenId, cancellationToken);
        if (current is null)
        {
            return null;
        }

        var updated = await tokenStore.UpsertAsync(
            current with
            {
                Status = TokenStatus.Revoked,
                RevokedAtUtc = DateTimeOffset.UtcNow,
                RevocationReason = request.Reason
            },
            cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "token.revoked",
                "token",
                updated.TokenId,
                request.ActorId,
                "success",
                $"Token was revoked for user '{updated.UserId}'. Reason: {request.Reason ?? "unspecified"}."),
            cancellationToken);

        return updated;
    }

    private static string CreateTokenId()
        => $"tok_{Guid.NewGuid():N}";

    private static string CreateRedeemSecret()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes);
    }

    private static string ComputeSecretHash(string redeemSecret)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(redeemSecret));
        return Convert.ToHexString(hash);
    }

    private static string? EvaluateRedeemability(TokenDto token)
    {
        if (token.Status == TokenStatus.Revoked)
        {
            return "Token redemption failed because the token is revoked.";
        }

        if (token.Status == TokenStatus.Redeemed)
        {
            return "Token redemption failed because the token is already redeemed.";
        }

        if (token.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            return "Token redemption failed because the token is expired.";
        }

        return null;
    }
}
