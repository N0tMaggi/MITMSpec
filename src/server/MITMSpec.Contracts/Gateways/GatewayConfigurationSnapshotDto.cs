namespace MITMSpec.Contracts.Gateways;

public sealed record GatewayConfigurationSnapshotDto(
    string SnapshotId,
    DateTimeOffset GeneratedAtUtc,
    string GatewayEndpoint,
    string GatewayPublicKey,
    string TunnelNetworkCidr,
    string DnsServer,
    string AllowedIps,
    int PersistentKeepaliveSeconds,
    IReadOnlyList<GatewayPeerAssignmentDto> PeerAssignments);
