using MITMSpec.Application.Abstractions;

namespace MITMSpec.Worker;

public class Worker(ILogger<Worker> logger, ISystemOverviewService systemOverviewService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var overview = await systemOverviewService.GetOverviewAsync(stoppingToken);
            logger.LogInformation(
                "MITMSpec worker heartbeat at {TimeUtc}. Events={TotalEvents}, Gateway={GatewayAgent}, Storage={PrimaryStorage}",
                DateTimeOffset.UtcNow,
                overview.TotalEvents,
                overview.GatewayAgent,
                overview.PrimaryStorage);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
