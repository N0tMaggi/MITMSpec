using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence.Configurations;

public sealed class AuditEntryEntityConfiguration : IEntityTypeConfiguration<AuditEntryEntity>
{
    public void Configure(EntityTypeBuilder<AuditEntryEntity> builder)
    {
        var converter = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.ToTable("audit_entries");
        builder.HasKey(item => item.AuditEntryId);
        builder.Property(item => item.OccurredAtUtc).HasConversion(converter);
        builder.Property(item => item.ActionType).HasMaxLength(64).IsRequired();
        builder.Property(item => item.SubjectType).HasMaxLength(32).IsRequired();
        builder.Property(item => item.SubjectId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.ActorId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.Result).HasMaxLength(32).IsRequired();
        builder.Property(item => item.Detail).HasMaxLength(1024).IsRequired();
        builder.HasIndex(item => item.OccurredAtUtc);
    }
}
