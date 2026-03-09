namespace MITMSpec.Application.Configuration;

public sealed class ProvisioningOptions
{
    public const string SectionName = "Provisioning";

    public string ControlPlaneBaseUrl { get; set; } = "https://localhost:5001";

    public string GatewayVpnEndpoint { get; set; } = "vpn.example.test:51820";

    public string GatewayVpnPublicKey { get; set; } = "REPLACE_WITH_GATEWAY_PUBLIC_KEY";

    public string GatewayTunnelNetworkCidr { get; set; } = "10.44.0.0/24";

    public string GatewayWireGuardInterface { get; set; } = "wg-mitmspec";

    public string ClientDnsServer { get; set; } = "1.1.1.1";

    public string AllowedIps { get; set; } = "0.0.0.0/0, ::/0";

    public int PersistentKeepaliveSeconds { get; set; } = 25;

    public string RootCaCommonName { get; set; } = "MITMSpec Root CA";
}
