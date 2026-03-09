using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence.Configurations;

public sealed class TrafficEventEntityConfiguration : IEntityTypeConfiguration<TrafficEventEntity>
{
    public void Configure(EntityTypeBuilder<TrafficEventEntity> builder)
    {
        var observedAtConverter = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.ToTable("traffic_events");
        builder.HasKey(item => item.EventId);

        builder.Property(item => item.EventId).HasMaxLength(128);
        builder.Property(item => item.ObservedAtUtc).HasConversion(observedAtConverter);
        builder.Property(item => item.GatewayId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.PeerId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.UserId).HasMaxLength(128).IsRequired();
        builder.Property(item => item.Scheme).HasMaxLength(16).IsRequired();
        builder.Property(item => item.Method).HasMaxLength(16).IsRequired();
        builder.Property(item => item.Host).HasMaxLength(256).IsRequired();
        builder.Property(item => item.Path).HasMaxLength(2048).IsRequired();
        builder.Property(item => item.MitmDisposition).HasMaxLength(64).IsRequired();
        builder.Property(item => item.BypassReason).HasMaxLength(256);
        builder.Property(item => item.TraceId).HasMaxLength(128).IsRequired();

        builder.HasIndex(item => item.ObservedAtUtc);
        builder.HasIndex(item => item.UserId);
        builder.HasIndex(item => new { item.GatewayId, item.PeerId });
    }
}
