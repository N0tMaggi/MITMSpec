using System.Collections.Concurrent;
using MITMSpec.Application.Abstractions;
using MITMSpec.Domain.Traffic;

namespace MITMSpec.Infrastructure.Stores;

public sealed class InMemoryTrafficEventStore : ITrafficEventStore
{
    private readonly ConcurrentDictionary<string, TrafficEvent> _events = new(StringComparer.Ordinal);

    public InMemoryTrafficEventStore()
    {
        var seed = new[]
        {
            new TrafficEvent(
                "evt-seed-001",
                DateTimeOffset.UtcNow.AddMinutes(-7),
                "gw-lab-01",
                "peer-lab-a",
                "user-alice",
                "https",
                "GET",
                "example.com",
                "/api/profile",
                200,
                "inspected",
                null,
                128,
                512,
                "{\"client\":\"lab\"}",
                "{\"ok\":true}",
                "trace-seed-001"),
            new TrafficEvent(
                "evt-seed-002",
                DateTimeOffset.UtcNow.AddMinutes(-2),
                "gw-lab-01",
                "peer-lab-b",
                "user-bob",
                "https",
                "POST",
                "api.internal.local",
                "/v1/webhooks",
                202,
                "inspected",
                null,
                421,
                84,
                "{\"type\":\"sample\"}",
                "{\"queued\":true}",
                "trace-seed-002")
        };

        foreach (var item in seed)
        {
            _events.TryAdd(item.EventId, item);
        }
    }

    public ValueTask<TrafficStoreWriteResult> AddAsync(TrafficEvent trafficEvent, CancellationToken cancellationToken = default)
    {
        var added = _events.TryAdd(trafficEvent.EventId, trafficEvent);
        return ValueTask.FromResult(added ? TrafficStoreWriteResult.Added : TrafficStoreWriteResult.Duplicate);
    }

    public ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(_events.Count);

    public ValueTask<TrafficEvent?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default)
    {
        _events.TryGetValue(eventId, out var trafficEvent);
        return ValueTask.FromResult(trafficEvent);
    }

    public ValueTask<IReadOnlyList<TrafficEvent>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        var recent = _events.Values
            .OrderByDescending(item => item.ObservedAtUtc)
            .Take(take)
            .ToArray();

        return ValueTask.FromResult<IReadOnlyList<TrafficEvent>>(recent);
    }
}
