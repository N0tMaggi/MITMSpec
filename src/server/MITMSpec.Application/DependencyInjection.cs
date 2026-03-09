using Microsoft.Extensions.DependencyInjection;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Services;

namespace MITMSpec.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISystemOverviewService, SystemOverviewService>();
        services.AddScoped<ITrafficIngestService, TrafficIngestService>();
        services.AddScoped<ITrafficQueryService, TrafficQueryService>();

        return services;
    }
}
