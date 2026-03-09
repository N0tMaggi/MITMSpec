using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence.Configurations;

public sealed class PeerEntityConfiguration : IEntityTypeConfiguration<PeerEntity>
{
    public void Configure(EntityTypeBuilder<PeerEntity> builder)
    {
        var converter = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.ToTable("peers");
        builder.HasKey(item => item.PeerId);
        builder.Property(item => item.PeerId).HasMaxLength(128);
        builder.Property(item => item.UserId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.EnrollmentTokenId).HasMaxLength(128);
        builder.Property(item => item.TunnelAddressCidr).HasMaxLength(64);
        builder.Property(item => item.ClientPublicKey).HasMaxLength(128);
        builder.Property(item => item.BoundAtUtc).HasConversion(converter);
        builder.Property(item => item.RemovedAtUtc).HasConversion(converter);
        builder.HasIndex(item => item.UserId);
        builder.HasIndex(item => item.TunnelAddressCidr).IsUnique();
    }
}
