using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MITMSpec.Infrastructure.Persistence;

namespace MITMSpec.IntegrationTests;

public sealed class TestWebApplicationFactory : WebApplicationFactory<MITMSpec.App.Program>, IDisposable
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"mitmspec-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Persistence:Provider", "Sqlite");
        builder.UseSetting("ConnectionStrings:Sqlite", $"Data Source={_databasePath}");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MITMSpecDbContext>>();
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return host;
    }

    public new void Dispose()
    {
        base.Dispose();

        try
        {
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
        catch (IOException)
        {
        }
    }
}
