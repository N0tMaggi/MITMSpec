using Microsoft.Extensions.Options;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Configuration;
using MITMSpec.Contracts.System;

namespace MITMSpec.Application.Services;

public sealed class SystemOverviewService(
    ITrafficEventStore trafficEventStore,
    IOptions<PlatformMetadataOptions> options) : ISystemOverviewService
{
    public async Task<SystemOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var metadata = options.Value;
        var totalEvents = await trafficEventStore.CountAsync(cancellationToken);

        return new SystemOverviewDto(
            metadata.PlatformName,
            metadata.ControlPlane,
            metadata.GatewayAgent,
            metadata.ProxyIntegration,
            metadata.PrimaryStorage,
            metadata.WindowsPackaging,
            totalEvents,
            DateTimeOffset.UtcNow,
            metadata.TopPriorities);
    }
}
