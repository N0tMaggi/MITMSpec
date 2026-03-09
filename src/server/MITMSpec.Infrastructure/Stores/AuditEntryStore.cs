using Microsoft.EntityFrameworkCore;
using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Audit;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Stores;

public sealed class AuditEntryStore(IDbContextFactory<MITMSpecDbContext> dbContextFactory) : IAuditEntryStore
{
    public async Task AddAsync(AuditEntryDto entry, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.AuditEntries.Add(new AuditEntryEntity
        {
            AuditEntryId = entry.AuditEntryId,
            OccurredAtUtc = entry.OccurredAtUtc,
            ActionType = entry.ActionType,
            SubjectType = entry.SubjectType,
            SubjectId = entry.SubjectId,
            ActorId = entry.ActorId,
            Result = entry.Result,
            Detail = entry.Detail
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditEntryDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var items = await dbContext.AuditEntries
            .AsNoTracking()
            .OrderByDescending(item => item.OccurredAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        return items.Select(item => new AuditEntryDto(
            item.AuditEntryId,
            item.OccurredAtUtc,
            item.ActionType,
            item.SubjectType,
            item.SubjectId,
            item.ActorId,
            item.Result,
            item.Detail)).ToArray();
    }
}
