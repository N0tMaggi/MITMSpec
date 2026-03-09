using MITMSpec.Contracts.Peers;

namespace MITMSpec.Application.Abstractions;

public interface IPeerStore
{
    Task<PeerDto> UpsertAsync(PeerDto peer, CancellationToken cancellationToken = default);

    Task<PeerDto?> GetByIdAsync(string peerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PeerDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetAllocatedTunnelAddressesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PeerDto>> GetActiveBindingsAsync(CancellationToken cancellationToken = default);
}
