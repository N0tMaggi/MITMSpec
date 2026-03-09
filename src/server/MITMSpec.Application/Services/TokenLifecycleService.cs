using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Services;

public sealed class TokenLifecycleService(ITokenStore tokenStore, IAuditEntryStore auditEntryStore) : ITokenLifecycleService
{
    public async Task<TokenDto> CreateAsync(CreateTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var token = new TokenDto(request.TokenId, request.UserId, "created", request.Description, DateTimeOffset.UtcNow, null, null);
        var saved = await tokenStore.UpsertAsync(token, cancellationToken);

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

        return saved;
    }

    public async Task<TokenDto?> RedeemAsync(string tokenId, TokenActionRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await tokenStore.GetByIdAsync(tokenId, cancellationToken);
        if (current is null)
        {
            return null;
        }

        var updated = await tokenStore.UpsertAsync(current with { Status = "redeemed", RedeemedAtUtc = DateTimeOffset.UtcNow }, cancellationToken);

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

        return updated;
    }

    public async Task<TokenDto?> RevokeAsync(string tokenId, TokenActionRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await tokenStore.GetByIdAsync(tokenId, cancellationToken);
        if (current is null)
        {
            return null;
        }

        var updated = await tokenStore.UpsertAsync(current with { Status = "revoked", RevokedAtUtc = DateTimeOffset.UtcNow }, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "token.revoked",
                "token",
                updated.TokenId,
                request.ActorId,
                "success",
                $"Token was revoked for user '{updated.UserId}'."),
            cancellationToken);

        return updated;
    }
}
