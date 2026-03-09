using MITMSpec.Contracts.Peers;

namespace MITMSpec.Application.Abstractions;

public interface IPeerStore
{
    Task<PeerDto> UpsertAsync(PeerDto peer, CancellationToken cancellationToken = default);

    Task<PeerDto?> GetByIdAsync(string peerId, CancellationToken cancellationToken = default);
}
