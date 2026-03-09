using MITMSpec.Domain.Traffic;

namespace MITMSpec.Application.Abstractions;

public interface ITrafficEventStore
{
    ValueTask<TrafficStoreWriteResult> AddAsync(TrafficEvent trafficEvent, CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TrafficEvent>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    ValueTask<TrafficEvent?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default);

    ValueTask<int> CountAsync(CancellationToken cancellationToken = default);
}
