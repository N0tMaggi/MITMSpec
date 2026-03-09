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

    public async Task<string?> GetSecretHashAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Tokens
            .AsNoTracking()
            .Where(item => item.TokenId == tokenId)
            .Select(item => item.SecretHash)
            .FirstOrDefaultAsync(cancellationToken);
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
        entity.ExpiresAtUtc = token.ExpiresAtUtc;
        entity.RedeemedAtUtc = token.RedeemedAtUtc;
        entity.RevokedAtUtc = token.RevokedAtUtc;
        entity.RevocationReason = token.RevocationReason;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<TokenDto> CreateAsync(TokenDto token, string secretHash, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = new TokenEntity
        {
            TokenId = token.TokenId,
            UserId = token.UserId,
            Status = token.Status,
            Description = token.Description,
            SecretHash = secretHash,
            CreatedAtUtc = token.CreatedAtUtc,
            ExpiresAtUtc = token.ExpiresAtUtc,
            RedeemedAtUtc = token.RedeemedAtUtc,
            RevokedAtUtc = token.RevokedAtUtc,
            RevocationReason = token.RevocationReason
        };

        dbContext.Tokens.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static TokenDto Map(TokenEntity entity) =>
        new(entity.TokenId, entity.UserId, entity.Status, entity.Description, entity.CreatedAtUtc, entity.ExpiresAtUtc, entity.RedeemedAtUtc, entity.RevokedAtUtc, entity.RevocationReason);
}
