using Microsoft.EntityFrameworkCore;
using MITMSpec.Infrastructure.Persistence.Entities;

namespace MITMSpec.Infrastructure.Persistence;

public sealed class MITMSpecDbContext(DbContextOptions<MITMSpecDbContext> options) : DbContext(options)
{
    public DbSet<AuditEntryEntity> AuditEntries => Set<AuditEntryEntity>();
    public DbSet<CertificateAuthorityEntity> CertificateAuthorities => Set<CertificateAuthorityEntity>();
    public DbSet<PeerEntity> Peers => Set<PeerEntity>();
    public DbSet<TokenEntity> Tokens => Set<TokenEntity>();
    public DbSet<TrafficEventEntity> TrafficEvents => Set<TrafficEventEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MITMSpecDbContext).Assembly);
    }
}
