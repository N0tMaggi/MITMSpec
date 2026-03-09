using Microsoft.EntityFrameworkCore;
using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Stores;

public sealed class TokenStore(IDbContextFactory<MITMSpecDbContext> dbContextFactory) : ITokenStore
{
    public async Task<TokenDto?> GetByIdAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Tokens.AsNoTracking().FirstOrDefaultAsync(item => item.TokenId == tokenId, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<TokenDto> UpsertAsync(TokenDto token, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Tokens.FirstOrDefaultAsync(item => item.TokenId == token.TokenId, cancellationToken);

        if (entity is null)
        {
            entity = new TokenEntity { TokenId = token.TokenId };
            dbContext.Tokens.Add(entity);
        }

        entity.UserId = token.UserId;
        entity.Status = token.Status;
        entity.Description = token.Description;
        entity.CreatedAtUtc = token.CreatedAtUtc;
        entity.RedeemedAtUtc = token.RedeemedAtUtc;
        entity.RevokedAtUtc = token.RevokedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static TokenDto Map(TokenEntity entity) =>
        new(entity.TokenId, entity.UserId, entity.Status, entity.Description, entity.CreatedAtUtc, entity.RedeemedAtUtc, entity.RevokedAtUtc);
}
