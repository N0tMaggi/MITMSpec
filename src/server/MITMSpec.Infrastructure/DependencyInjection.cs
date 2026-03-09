using Microsoft.Extensions.DependencyInjection;
using MITMSpec.Application.Abstractions;
using MITMSpec.Infrastructure.Stores;

namespace MITMSpec.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITrafficEventStore, InMemoryTrafficEventStore>();
        return services;
    }
}
