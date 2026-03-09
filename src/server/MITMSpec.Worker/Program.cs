using MITMSpec.Application;
using MITMSpec.Application.Configuration;
using MITMSpec.Infrastructure;

namespace MITMSpec.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<PlatformMetadataOptions>(builder.Configuration.GetSection(PlatformMetadataOptions.SectionName));
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}
