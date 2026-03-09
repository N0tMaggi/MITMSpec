using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MITMSpec.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MITMSpecDbContext>
{
    public MITMSpecDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSql") ??
            "Host=localhost;Port=5432;Database=mitmspec;Username=mitmspec;Password=mitmspec";

        var optionsBuilder = new DbContextOptionsBuilder<MITMSpecDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsAssembly(typeof(MITMSpecDbContext).Assembly.FullName);
        });

        return new MITMSpecDbContext(optionsBuilder.Options);
    }
}
