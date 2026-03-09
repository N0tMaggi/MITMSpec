using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence.Configurations;

public sealed class CertificateAuthorityEntityConfiguration : IEntityTypeConfiguration<CertificateAuthorityEntity>
{
    public void Configure(EntityTypeBuilder<CertificateAuthorityEntity> builder)
    {
        var converter = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.ToTable("certificate_authorities");
        builder.HasKey(item => item.CertificateAuthorityId);
        builder.Property(item => item.CertificateAuthorityId).HasMaxLength(128);
        builder.Property(item => item.CommonName).HasMaxLength(256).IsRequired();
        builder.Property(item => item.Thumbprint).HasMaxLength(128).IsRequired();
        builder.Property(item => item.SerialNumber).HasMaxLength(128).IsRequired();
        builder.Property(item => item.CertificatePem).IsRequired();
        builder.Property(item => item.EncryptedPrivateKeyPem).IsRequired();
        builder.Property(item => item.CreatedAtUtc).HasConversion(converter);
        builder.Property(item => item.ActivatedAtUtc).HasConversion(converter);
        builder.Property(item => item.RotatedAtUtc).HasConversion(converter);
        builder.HasIndex(item => item.IsActive);
        builder.HasIndex(item => item.Thumbprint).IsUnique();
    }
}
