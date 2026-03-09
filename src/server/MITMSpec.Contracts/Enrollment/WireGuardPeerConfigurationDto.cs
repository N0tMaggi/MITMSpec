namespace MITMSpec.Contracts.Enrollment;

public sealed record WireGuardPeerConfigurationDto(
    string AssignedAddressCidr,
    string DnsServer,
    string GatewayEndpoint,
    string GatewayPublicKey,
    string AllowedIps,
    int PersistentKeepaliveSeconds);
