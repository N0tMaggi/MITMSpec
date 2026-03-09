using MITMSpec.Contracts.Peers;

namespace MITMSpec.Application.Abstractions;

public interface IPeerQueryService
{
    Task<IReadOnlyList<PeerDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
