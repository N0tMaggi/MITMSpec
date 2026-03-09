using MITMSpec.Application.Abstractions;

namespace MITMSpec.Worker;

public class Worker(ILogger<Worker> logger, ISystemOverviewService systemOverviewService) : BackgroundService
{
    private static readonly Action<ILogger, DateTimeOffset, int, string, string, Exception?> HeartbeatLog =
        LoggerMessage.Define<DateTimeOffset, int, string, string>(
            LogLevel.Information,
            new EventId(1000, "WorkerHeartbeat"),
            "MITMSpec worker heartbeat at {TimeUtc}. Events={TotalEvents}, Gateway={GatewayAgent}, Storage={PrimaryStorage}");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var overview = await systemOverviewService.GetOverviewAsync(stoppingToken);
            HeartbeatLog(
                logger,
                DateTimeOffset.UtcNow,
                overview.TotalEvents,
                overview.GatewayAgent,
                overview.PrimaryStorage,
                null);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
