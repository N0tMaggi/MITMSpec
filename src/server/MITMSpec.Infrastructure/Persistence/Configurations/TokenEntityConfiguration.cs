using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence.Configurations;

public sealed class TokenEntityConfiguration : IEntityTypeConfiguration<TokenEntity>
{
    public void Configure(EntityTypeBuilder<TokenEntity> builder)
    {
        var converter = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.ToTable("tokens");
        builder.HasKey(item => item.TokenId);
        builder.Property(item => item.TokenId).HasMaxLength(128);
        builder.Property(item => item.UserId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(TokenStatus.Pending)
            .IsRequired();
        builder.Property(item => item.Description).HasMaxLength(256).IsRequired();
        builder.Property(item => item.SecretHash).HasMaxLength(128).IsRequired();
        builder.Property(item => item.CreatedAtUtc).HasConversion(converter);
        builder.Property(item => item.ExpiresAtUtc).HasConversion(converter);
        builder.Property(item => item.RedeemedAtUtc).HasConversion(converter);
        builder.Property(item => item.RevokedAtUtc).HasConversion(converter);
        builder.Property(item => item.RevocationReason).HasMaxLength(256);
        builder.HasIndex(item => item.UserId);
    }
}
