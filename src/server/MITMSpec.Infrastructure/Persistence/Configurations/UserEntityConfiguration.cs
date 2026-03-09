using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence.Configurations;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        var converter = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.ToTable("users");
        builder.HasKey(item => item.UserId);
        builder.Property(item => item.UserId).HasMaxLength(128);
        builder.Property(item => item.DisplayName).HasMaxLength(256).IsRequired();
        builder.Property(item => item.CreatedAtUtc).HasConversion(converter);
        builder.Property(item => item.DeactivatedAtUtc).HasConversion(converter);
    }
}
