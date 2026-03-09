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

    public async Task<IReadOnlyList<PeerDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var items = await dbContext.Peers
            .AsNoTracking()
            .OrderByDescending(item => item.BoundAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToArray();
    }

    public async Task<IReadOnlyList<PeerDto>> GetActiveBindingsAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var items = await dbContext.Peers
            .AsNoTracking()
            .Where(item => item.IsBound && item.RemovedAtUtc == null)
            .OrderBy(item => item.PeerId)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToArray();
    }

    public async Task<IReadOnlyList<string>> GetAllocatedTunnelAddressesAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var items = await dbContext.Peers
            .AsNoTracking()
            .Where(item => item.IsBound && item.RemovedAtUtc == null && item.TunnelAddressCidr != null)
            .Select(item => item.TunnelAddressCidr!)
            .ToListAsync(cancellationToken);

        return items;
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
        entity.EnrollmentTokenId = peer.EnrollmentTokenId;
        entity.TunnelAddressCidr = peer.TunnelAddressCidr;
        entity.ClientPublicKey = peer.ClientPublicKey;
        entity.IsBound = peer.IsBound;
        entity.BoundAtUtc = peer.BoundAtUtc;
        entity.RemovedAtUtc = peer.RemovedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static PeerDto Map(PeerEntity entity) =>
        new(entity.PeerId, entity.UserId, entity.EnrollmentTokenId, entity.TunnelAddressCidr, entity.ClientPublicKey, entity.IsBound, entity.BoundAtUtc, entity.RemovedAtUtc);
}
