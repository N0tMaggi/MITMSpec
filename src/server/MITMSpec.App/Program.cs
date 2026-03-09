using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MITMSpec.App.Api;
using MITMSpec.App.Components;
using MITMSpec.Application;
using MITMSpec.Application.Configuration;
using MITMSpec.Infrastructure;
using MITMSpec.Infrastructure.Persistence;

namespace MITMSpec.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<PlatformMetadataOptions>(builder.Configuration.GetSection(PlatformMetadataOptions.SectionName));
        builder.Services.Configure<ProvisioningOptions>(builder.Configuration.GetSection(ProvisioningOptions.SectionName));
        builder.Services.AddDataProtection();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks();
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("api", limiterOptions =>
            {
                limiterOptions.PermitLimit = 120;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        var app = builder.Build();

        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MITMSpecDbContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();
            dbContext.Database.Migrate();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler();
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.UseRateLimiter();

        app.MapOpenApi();
        app.MapHealthChecks("/health/live");
        app.MapHealthChecks("/health/ready");
        app.MapMITMSpecApi();
        app.MapStaticAssets();
        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
