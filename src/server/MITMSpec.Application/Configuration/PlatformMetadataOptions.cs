namespace MITMSpec.Application.Configuration;

public sealed class PlatformMetadataOptions
{
    public const string SectionName = "PlatformMetadata";

    public string PlatformName { get; set; } = "MITMSpec";

    public string ControlPlane { get; set; } = "ASP.NET Core 10 modular monolith";

    public string GatewayAgent { get; set; } = "Go host agent";

    public string ProxyIntegration { get; set; } = "Python mitmproxy addon";

    public string PrimaryStorage { get; set; } = "PostgreSQL 16+";

    public string WindowsPackaging { get; set; } = "WiX v4 MSI + EXE bootstrapper";

    public List<string> TopPriorities { get; set; } = [];
}
