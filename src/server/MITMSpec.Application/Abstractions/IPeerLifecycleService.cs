using MITMSpec.Contracts.Peers;

namespace MITMSpec.Application.Abstractions;

public interface IPeerLifecycleService
{
    Task<PeerDto> BindPeerAsync(BindPeerRequestDto request, CancellationToken cancellationToken = default);

    Task<PeerDto?> RemovePeerAsync(string peerId, RemovePeerRequestDto request, CancellationToken cancellationToken = default);
}
