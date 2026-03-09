using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MITMSpec.Application.Abstractions;
using MITMSpec.Infrastructure.Persistence;
using MITMSpec.Infrastructure.Stores;

namespace MITMSpec.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var provider = configuration["Persistence:Provider"] ?? "PostgreSql";

        services.AddDbContextFactory<MITMSpecDbContext>(options =>
        {
            switch (provider)
            {
                case "Sqlite":
                    var sqliteConnectionString = configuration.GetConnectionString("Sqlite");
                    if (string.IsNullOrWhiteSpace(sqliteConnectionString))
                    {
                        throw new InvalidOperationException("Connection string 'Sqlite' is required when Persistence:Provider is 'Sqlite'.");
                    }

                    options.UseSqlite(sqliteConnectionString, sqlite =>
                    {
                        sqlite.MigrationsAssembly(typeof(MITMSpecDbContext).Assembly.FullName);
                    });
                    break;

                default:
                    var postgresConnectionString = configuration.GetConnectionString("PostgreSql");
                    if (string.IsNullOrWhiteSpace(postgresConnectionString))
                    {
                        throw new InvalidOperationException("Connection string 'PostgreSql' is required.");
                    }

                    options.UseNpgsql(postgresConnectionString, npgsql =>
                    {
                        npgsql.MigrationsAssembly(typeof(MITMSpecDbContext).Assembly.FullName);
                    });
                    break;
            }

            if (environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });

        services.AddScoped<ITrafficEventStore, TrafficEventStore>();
        services.AddScoped<IAuditEntryStore, AuditEntryStore>();
        services.AddScoped<ICertificateAuthorityStore, CertificateAuthorityStore>();
        services.AddScoped<IUserStore, UserStore>();
        services.AddScoped<ITokenStore, TokenStore>();
        services.AddScoped<IPeerStore, PeerStore>();
        return services;
    }
}
