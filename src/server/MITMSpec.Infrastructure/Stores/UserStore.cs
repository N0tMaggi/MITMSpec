using Microsoft.EntityFrameworkCore;
using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Users;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Stores;

public sealed class UserStore(IDbContextFactory<MITMSpecDbContext> dbContextFactory) : IUserStore
{
    public async Task<UserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<UserDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var items = await dbContext.Users
            .AsNoTracking()
            .OrderByDescending(item => item.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToArray();
    }

    public async Task<UserDto> UpsertAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Users.FirstOrDefaultAsync(item => item.UserId == user.UserId, cancellationToken);

        if (entity is null)
        {
            entity = new UserEntity { UserId = user.UserId };
            dbContext.Users.Add(entity);
        }

        entity.DisplayName = user.DisplayName;
        entity.IsActive = user.IsActive;
        entity.CreatedAtUtc = user.CreatedAtUtc;
        entity.DeactivatedAtUtc = user.DeactivatedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static UserDto Map(UserEntity entity) =>
        new(entity.UserId, entity.DisplayName, entity.IsActive, entity.CreatedAtUtc, entity.DeactivatedAtUtc);
}
