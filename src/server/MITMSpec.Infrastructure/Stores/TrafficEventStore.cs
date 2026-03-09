using Microsoft.EntityFrameworkCore;
using MITMSpec.Application.Abstractions;
using MITMSpec.Domain.Traffic;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Stores;

public sealed class TrafficEventStore(IDbContextFactory<MITMSpecDbContext> dbContextFactory) : ITrafficEventStore
{
    public async ValueTask<TrafficStoreWriteResult> AddAsync(TrafficEvent trafficEvent, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var exists = await dbContext.TrafficEvents
            .AsNoTracking()
            .AnyAsync(item => item.EventId == trafficEvent.EventId, cancellationToken);

        if (exists)
        {
            return TrafficStoreWriteResult.Duplicate;
        }

        dbContext.TrafficEvents.Add(MapToEntity(trafficEvent));
        await dbContext.SaveChangesAsync(cancellationToken);

        return TrafficStoreWriteResult.Added;
    }

    public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.TrafficEvents.CountAsync(cancellationToken);
    }

    public async ValueTask<TrafficEvent?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await dbContext.TrafficEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.EventId == eventId, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public async ValueTask<IReadOnlyList<TrafficEvent>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var events = await dbContext.TrafficEvents
            .AsNoTracking()
            .OrderByDescending(item => item.ObservedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        return events.Select(MapToDomain).ToArray();
    }

    private static TrafficEventEntity MapToEntity(TrafficEvent trafficEvent) =>
        new()
        {
            EventId = trafficEvent.EventId,
            ObservedAtUtc = trafficEvent.ObservedAtUtc,
            GatewayId = trafficEvent.GatewayId,
            PeerId = trafficEvent.PeerId,
            UserId = trafficEvent.UserId,
            Scheme = trafficEvent.Scheme,
            Method = trafficEvent.Method,
            Host = trafficEvent.Host,
            Path = trafficEvent.Path,
            StatusCode = trafficEvent.StatusCode,
            MitmDisposition = trafficEvent.MitmDisposition,
            BypassReason = trafficEvent.BypassReason,
            RequestBodyBytes = trafficEvent.RequestBodyBytes,
            ResponseBodyBytes = trafficEvent.ResponseBodyBytes,
            RequestBody = trafficEvent.RequestBody,
            ResponseBody = trafficEvent.ResponseBody,
            TraceId = trafficEvent.TraceId
        };

    private static TrafficEvent MapToDomain(TrafficEventEntity entity) =>
        new(
            entity.EventId,
            entity.ObservedAtUtc,
            entity.GatewayId,
            entity.PeerId,
            entity.UserId,
            entity.Scheme,
            entity.Method,
            entity.Host,
            entity.Path,
            entity.StatusCode,
            entity.MitmDisposition,
            entity.BypassReason,
            entity.RequestBodyBytes,
            entity.ResponseBodyBytes,
            entity.RequestBody,
            entity.ResponseBody,
            entity.TraceId);
}
