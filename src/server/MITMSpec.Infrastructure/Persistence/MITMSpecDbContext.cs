using Microsoft.EntityFrameworkCore;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence;

public sealed class MITMSpecDbContext(DbContextOptions<MITMSpecDbContext> options) : DbContext(options)
{
    public DbSet<TrafficEventEntity> TrafficEvents => Set<TrafficEventEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MITMSpecDbContext).Assembly);
    }
}
