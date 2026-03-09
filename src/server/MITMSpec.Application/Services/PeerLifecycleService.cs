using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Peers;

namespace MITMSpec.Application.Services;

public sealed class PeerLifecycleService(IPeerStore peerStore, IAuditEntryStore auditEntryStore) : IPeerLifecycleService
{
    public async Task<PeerDto> BindPeerAsync(BindPeerRequestDto request, CancellationToken cancellationToken = default)
    {
        var peer = new PeerDto(request.PeerId, request.UserId, null, null, null, true, DateTimeOffset.UtcNow, null);
        var saved = await peerStore.UpsertAsync(peer, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "peer.bound",
                "peer",
                saved.PeerId,
                request.ActorId,
                "success",
                $"Peer '{saved.PeerId}' was bound to user '{saved.UserId}'."),
            cancellationToken);

        return saved;
    }

    public async Task<PeerDto?> RemovePeerAsync(string peerId, RemovePeerRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await peerStore.GetByIdAsync(peerId, cancellationToken);
        if (current is null)
        {
            return null;
        }

        var updated = await peerStore.UpsertAsync(current with { IsBound = false, RemovedAtUtc = DateTimeOffset.UtcNow }, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "peer.removed",
                "peer",
                updated.PeerId,
                request.ActorId,
                "success",
                $"Peer '{updated.PeerId}' was removed. Reason: {request.Reason}."),
            cancellationToken);

        return updated;
    }
}
