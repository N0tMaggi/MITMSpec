using Microsoft.EntityFrameworkCore;
using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Certificates;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Stores;

public sealed class CertificateAuthorityStore(IDbContextFactory<MITMSpecDbContext> dbContextFactory) : ICertificateAuthorityStore
{
    public async Task<CertificateAuthorityDto?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.CertificateAuthorities
            .AsNoTracking()
            .OrderByDescending(item => item.ActivatedAtUtc)
            .FirstOrDefaultAsync(item => item.IsActive, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<CertificateAuthorityDto> AddAsync(
        CertificateAuthorityDto authority,
        string encryptedPrivateKeyPem,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = new CertificateAuthorityEntity
        {
            CertificateAuthorityId = authority.CertificateAuthorityId,
            CommonName = authority.CommonName,
            Thumbprint = authority.Thumbprint,
            SerialNumber = authority.SerialNumber,
            CertificatePem = authority.CertificatePem,
            EncryptedPrivateKeyPem = encryptedPrivateKeyPem,
            CreatedAtUtc = authority.CreatedAtUtc,
            ActivatedAtUtc = authority.ActivatedAtUtc,
            IsActive = authority.IsActive,
            RotatedAtUtc = authority.RotatedAtUtc
        };

        dbContext.CertificateAuthorities.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task DeactivateAsync(string certificateAuthorityId, DateTimeOffset rotatedAtUtc, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.CertificateAuthorities.FirstOrDefaultAsync(
            item => item.CertificateAuthorityId == certificateAuthorityId,
            cancellationToken);

        if (entity is null)
        {
            return;
        }

        entity.IsActive = false;
        entity.RotatedAtUtc = rotatedAtUtc;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CertificateAuthorityDto Map(CertificateAuthorityEntity entity) =>
        new(
            entity.CertificateAuthorityId,
            entity.CommonName,
            entity.Thumbprint,
            entity.SerialNumber,
            entity.CertificatePem,
            entity.CreatedAtUtc,
            entity.ActivatedAtUtc,
            entity.IsActive,
            entity.RotatedAtUtc);
}
