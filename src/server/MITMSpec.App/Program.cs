using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using MITMSpec.App.Api;
using MITMSpec.App.Components;
using MITMSpec.Application;
using MITMSpec.Application.Configuration;
using MITMSpec.Infrastructure;

namespace MITMSpec.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<PlatformMetadataOptions>(builder.Configuration.GetSection(PlatformMetadataOptions.SectionName));
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure();
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
