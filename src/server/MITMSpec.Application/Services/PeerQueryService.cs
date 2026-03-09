using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Peers;

namespace MITMSpec.Application.Services;

public sealed class PeerQueryService(IPeerStore peerStore) : IPeerQueryService
{
    public Task<IReadOnlyList<PeerDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
        => peerStore.GetRecentAsync(Math.Clamp(take, 1, 200), cancellationToken);
}
