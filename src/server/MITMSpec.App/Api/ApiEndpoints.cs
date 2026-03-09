using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Traffic;

namespace MITMSpec.App.Api;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapMITMSpecApi(this IEndpointRouteBuilder endpoints)
    {
        var system = endpoints.MapGroup("/api/system").RequireRateLimiting("api").WithTags("System");
        system.MapGet("/overview", async (ISystemOverviewService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetOverviewAsync(cancellationToken)))
            .WithName("GetSystemOverview")
            .WithSummary("Gets the current MITMSpec platform overview.");

        var traffic = endpoints.MapGroup("/api/traffic").RequireRateLimiting("api").WithTags("Traffic");
        traffic.MapGet("/events", async (ITrafficQueryService service, int? take, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetRecentAsync(take ?? 25, cancellationToken)))
            .WithName("GetRecentTrafficEvents")
            .WithSummary("Gets recent traffic events for the operator dashboard.");

        traffic.MapGet("/events/{eventId}", async Task<IResult> (string eventId, ITrafficQueryService service, CancellationToken cancellationToken) =>
        {
            var detail = await service.GetByIdAsync(eventId, cancellationToken);
            return detail is null ? TypedResults.NotFound() : TypedResults.Ok(detail);
        })
        .WithName("GetTrafficEventById")
        .WithSummary("Gets a traffic event by event id.");

        var ingest = endpoints.MapGroup("/ingest/traffic").RequireRateLimiting("api").WithTags("Ingest");
        ingest.MapPost("/", async Task<IResult> (TrafficEnvelopeV1 envelope, ITrafficIngestService service, CancellationToken cancellationToken) =>
        {
            var result = await service.IngestAsync(envelope, cancellationToken);

            return result.Outcome switch
            {
                TrafficIngestOutcome.Accepted => TypedResults.Ok(result),
                TrafficIngestOutcome.Duplicate => TypedResults.Conflict(result),
                _ => TypedResults.BadRequest(result)
            };
        })
        .WithName("IngestTrafficEnvelope")
        .WithSummary("Accepts a normalized traffic envelope from the gateway/proxy path.");

        endpoints.MapPlaceholderGroup("/api/auth", "Authentication flows are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/users", "User management endpoints are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/tokens", "Provisioning token endpoints are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/peers", "Peer lifecycle endpoints are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/gateways", "Gateway management endpoints are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/webhooks", "Webhook endpoints are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/audit", "Audit endpoints are planned for a later phase.");

        return endpoints;
    }

    private static void MapPlaceholderGroup(this IEndpointRouteBuilder endpoints, string prefix, string detail)
    {
        endpoints.MapGroup(prefix)
            .RequireRateLimiting("api")
            .MapGet("/", () => Results.Problem(
                title: "Not implemented",
                detail: detail,
                statusCode: StatusCodes.Status501NotImplemented))
            .WithSummary(detail);
    }
}
