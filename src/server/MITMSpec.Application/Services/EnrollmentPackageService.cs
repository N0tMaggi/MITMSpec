using Microsoft.Extensions.Options;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Configuration;
using MITMSpec.Contracts.Enrollment;
using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Services;

public sealed class EnrollmentPackageService(
    ICertificateAuthorityService certificateAuthorityService,
    ITokenLifecycleService tokenLifecycleService,
    IOptions<ProvisioningOptions> provisioningOptions) : IEnrollmentPackageService
{
    public async Task<EnrollmentPackageDto> CreateAsync(CreateEnrollmentPackageRequestDto request, CancellationToken cancellationToken = default)
    {
        var authority = await certificateAuthorityService.GetActiveAsync(cancellationToken);
        var issuedToken = await tokenLifecycleService.CreateAsync(
            new CreateTokenRequestDto(
                request.ActorId,
                request.UserId,
                request.Description,
                request.LifetimeHours),
            cancellationToken);

        return new EnrollmentPackageDto(
            $"pkg_{Guid.NewGuid():N}",
            DateTimeOffset.UtcNow,
            issuedToken,
            authority,
            provisioningOptions.Value.ControlPlaneBaseUrl.TrimEnd('/'),
            $"/api/tokens/{issuedToken.Token.TokenId}/redeem",
            BuildWireGuardTemplate(),
            [
                "Install the MITMSpec root CA certificate on the client before HTTPS inspection.",
                "Treat the redeem secret as a one-time bootstrap secret and do not store it in plaintext after redemption.",
                "Generate WireGuard client keys on the client device. The current template is a bootstrap template and not a fully assigned peer config."
            ]);
    }

    private string BuildWireGuardTemplate()
    {
        var options = provisioningOptions.Value;
        return string.Join(
            Environment.NewLine,
            [
                "# MITMSpec WireGuard bootstrap template",
                "# Generate the client keypair locally and replace placeholders before first use.",
                "[Interface]",
                "PrivateKey = <generate-locally>",
                "Address = <assigned-by-gateway-after-enrollment>",
                $"DNS = {options.ClientDnsServer}",
                string.Empty,
                "[Peer]",
                $"PublicKey = {options.GatewayVpnPublicKey}",
                $"Endpoint = {options.GatewayVpnEndpoint}",
                $"AllowedIPs = {options.AllowedIps}",
                "PersistentKeepalive = 25"
            ]);
    }
}
