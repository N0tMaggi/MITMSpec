using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Traffic;

namespace MITMSpec.Application.Services;

public sealed class TrafficQueryService(ITrafficEventStore trafficEventStore) : ITrafficQueryService
{
    public async Task<IReadOnlyList<TrafficEventSummaryDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        var events = await trafficEventStore.GetRecentAsync(Math.Clamp(take, 1, 100), cancellationToken);

        return events
            .Select(item => new TrafficEventSummaryDto(
                item.EventId,
                item.ObservedAtUtc,
                item.GatewayId,
                item.PeerId,
                item.UserId,
                item.Method,
                item.Host,
                item.Path,
                item.StatusCode,
                item.MitmDisposition,
                item.HasBodies))
            .ToArray();
    }

    public async Task<TrafficEventDetailDto?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default)
    {
        var item = await trafficEventStore.GetByIdAsync(eventId, cancellationToken);
        if (item is null)
        {
            return null;
        }

        return new TrafficEventDetailDto(
            item.EventId,
            item.ObservedAtUtc,
            item.GatewayId,
            item.PeerId,
            item.UserId,
            item.Scheme,
            item.Method,
            item.Host,
            item.Path,
            item.StatusCode,
            item.MitmDisposition,
            item.BypassReason,
            item.RequestBodyBytes,
            item.ResponseBodyBytes,
            item.RequestBody,
            item.ResponseBody,
            item.TraceId);
    }
}
