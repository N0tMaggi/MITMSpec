using Microsoft.Extensions.DependencyInjection;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Services;

namespace MITMSpec.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ICertificateAuthorityService, CertificateAuthorityService>();
        services.AddScoped<IEnrollmentPackageService, EnrollmentPackageService>();
        services.AddScoped<IGatewayConfigurationService, GatewayConfigurationService>();
        services.AddScoped<IPeerAddressAllocator, PeerAddressAllocator>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<ITokenQueryService, TokenQueryService>();
        services.AddScoped<IPeerQueryService, PeerQueryService>();
        services.AddScoped<ISystemOverviewService, SystemOverviewService>();
        services.AddScoped<IUserLifecycleService, UserLifecycleService>();
        services.AddScoped<ITokenLifecycleService, TokenLifecycleService>();
        services.AddScoped<IPeerLifecycleService, PeerLifecycleService>();
        services.AddScoped<ITrafficIngestService, TrafficIngestService>();
        services.AddScoped<ITrafficQueryService, TrafficQueryService>();

        return services;
    }
}
