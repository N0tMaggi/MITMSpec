using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Services;
using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.Traffic;
using MITMSpec.Domain.Traffic;

namespace MITMSpec.UnitTests;

public class TrafficIngestServiceTests
{
    [Fact]
    public async Task IngestAsyncQuarantinesEnvelopeWhenPeerCannotBeResolved()
    {
        var service = new TrafficIngestService(new FakeTrafficEventStore(), new FakePeerStore(null));
        var envelope = CreateEnvelope();

        var result = await service.IngestAsync(envelope);

        Assert.Equal(TrafficIngestOutcome.Quarantined, result.Outcome);
    }

    [Fact]
    public async Task IngestAsyncAcceptsEnvelopeWhenEnvelopeMatchesBoundPeer()
    {
        var peer = new PeerDto("peer-test", "user-test", "tok-test", true, DateTimeOffset.UtcNow, null);
        var service = new TrafficIngestService(new FakeTrafficEventStore(), new FakePeerStore(peer));

        var result = await service.IngestAsync(CreateEnvelope());

        Assert.Equal(TrafficIngestOutcome.Accepted, result.Outcome);
    }

    [Fact]
    public async Task IngestAsyncQuarantinesEnvelopeWhenEnvelopeUserDoesNotMatchBoundPeer()
    {
        var peer = new PeerDto("peer-test", "user-other", "tok-test", true, DateTimeOffset.UtcNow, null);
        var service = new TrafficIngestService(new FakeTrafficEventStore(), new FakePeerStore(peer));

        var result = await service.IngestAsync(CreateEnvelope());

        Assert.Equal(TrafficIngestOutcome.Quarantined, result.Outcome);
    }

    private static TrafficEnvelopeV1 CreateEnvelope() =>
        new(
            "evt-test-001",
            DateTimeOffset.UtcNow,
            "gw-test",
            "peer-test",
            "user-test",
            "https",
            "GET",
            "example.test",
            "/resource",
            200,
            "inspected",
            null,
            0,
            0,
            null,
            null,
            "trace-test-001");

    private sealed class FakeTrafficEventStore : ITrafficEventStore
    {
        public ValueTask<TrafficStoreWriteResult> AddAsync(TrafficEvent trafficEvent, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(TrafficStoreWriteResult.Added);

        public ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult(0);

        public ValueTask<TrafficEvent?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default)
            => ValueTask.FromResult<TrafficEvent?>(null);

        public ValueTask<IReadOnlyList<TrafficEvent>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
            => ValueTask.FromResult<IReadOnlyList<TrafficEvent>>([]);
    }

    private sealed class FakePeerStore(PeerDto? peer) : IPeerStore
    {
        public Task<PeerDto?> GetByIdAsync(string peerId, CancellationToken cancellationToken = default)
            => Task.FromResult(peer?.PeerId == peerId ? peer : null);

        public Task<PeerDto> UpsertAsync(PeerDto model, CancellationToken cancellationToken = default)
            => Task.FromResult(model);
    }
}
