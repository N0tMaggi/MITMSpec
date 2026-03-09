using MITMSpec.Contracts.Traffic;

namespace MITMSpec.Application.Abstractions;

public interface ITrafficQueryService
{
    Task<IReadOnlyList<TrafficEventSummaryDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<TrafficEventDetailDto?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default);
}
