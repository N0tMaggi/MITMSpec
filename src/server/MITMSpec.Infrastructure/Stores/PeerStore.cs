using Microsoft.EntityFrameworkCore;
using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Peers;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Stores;

public sealed class PeerStore(IDbContextFactory<MITMSpecDbContext> dbContextFactory) : IPeerStore
{
    public async Task<PeerDto?> GetByIdAsync(string peerId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Peers.AsNoTracking().FirstOrDefaultAsync(item => item.PeerId == peerId, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<PeerDto> UpsertAsync(PeerDto peer, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Peers.FirstOrDefaultAsync(item => item.PeerId == peer.PeerId, cancellationToken);

        if (entity is null)
        {
            entity = new PeerEntity { PeerId = peer.PeerId };
            dbContext.Peers.Add(entity);
        }

        entity.UserId = peer.UserId;
        entity.IsBound = peer.IsBound;
        entity.BoundAtUtc = peer.BoundAtUtc;
        entity.RemovedAtUtc = peer.RemovedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static PeerDto Map(PeerEntity entity) =>
        new(entity.PeerId, entity.UserId, entity.IsBound, entity.BoundAtUtc, entity.RemovedAtUtc);
}
