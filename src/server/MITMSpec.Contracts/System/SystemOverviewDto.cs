namespace MITMSpec.Contracts.System;

public sealed record SystemOverviewDto(
    string PlatformName,
    string ControlPlane,
    string GatewayAgent,
    string ProxyIntegration,
    string PrimaryStorage,
    string WindowsPackaging,
    int TotalEvents,
    DateTimeOffset LastUpdatedUtc,
    IReadOnlyList<string> TopPriorities);
