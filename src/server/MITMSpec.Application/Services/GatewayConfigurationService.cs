using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Configuration;
using MITMSpec.Contracts.Gateways;

namespace MITMSpec.Application.Services;

public sealed class GatewayConfigurationService(
    IPeerStore peerStore,
    IOptions<ProvisioningOptions> provisioningOptions) : IGatewayConfigurationService
{
    public async Task<IReadOnlyList<GatewayPeerAssignmentDto>> GetPeerAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var peers = await peerStore.GetActiveBindingsAsync(cancellationToken);

        return peers
            .Where(peer => !string.IsNullOrWhiteSpace(peer.TunnelAddressCidr) && !string.IsNullOrWhiteSpace(peer.ClientPublicKey))
            .OrderBy(peer => peer.PeerId, StringComparer.Ordinal)
            .Select(peer => new GatewayPeerAssignmentDto(
                peer.PeerId,
                peer.UserId,
                peer.TunnelAddressCidr!,
                peer.ClientPublicKey!,
                peer.EnrollmentTokenId,
                peer.BoundAtUtc))
            .ToArray();
    }

    public async Task<GatewayConfigurationSnapshotDto> GetCurrentSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var options = provisioningOptions.Value;
        var assignments = await GetPeerAssignmentsAsync(cancellationToken);
        var generatedAt = DateTimeOffset.UtcNow;

        return new GatewayConfigurationSnapshotDto(
            ComputeSnapshotId(options, assignments),
            generatedAt,
            options.GatewayVpnEndpoint,
            options.GatewayVpnPublicKey,
            options.GatewayTunnelNetworkCidr,
            options.ClientDnsServer,
            options.AllowedIps,
            options.PersistentKeepaliveSeconds,
            assignments);
    }

    private static string ComputeSnapshotId(ProvisioningOptions options, IReadOnlyList<GatewayPeerAssignmentDto> assignments)
    {
        var builder = new StringBuilder()
            .Append(options.GatewayVpnEndpoint).Append('|')
            .Append(options.GatewayVpnPublicKey).Append('|')
            .Append(options.GatewayTunnelNetworkCidr).Append('|')
            .Append(options.ClientDnsServer).Append('|')
            .Append(options.AllowedIps).Append('|')
            .Append(options.PersistentKeepaliveSeconds);

        foreach (var assignment in assignments)
        {
            builder.Append('|')
                .Append(assignment.PeerId).Append(':')
                .Append(assignment.UserId).Append(':')
                .Append(assignment.TunnelAddressCidr).Append(':')
                .Append(assignment.ClientPublicKey);
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString()));
        return Convert.ToHexString(bytes);
    }
}
