using MITMSpec.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using MITMSpec.Contracts.Auth;
using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Contracts.Traffic;
using MITMSpec.Contracts.Users;

namespace MITMSpec.App.Api;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapMITMSpecApi(this IEndpointRouteBuilder endpoints)
    {
        var system = endpoints.MapGroup("/api/system").RequireRateLimiting("api").WithTags("System");
        system.MapGet("/overview", async ([FromServices] ISystemOverviewService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetOverviewAsync(cancellationToken)))
            .WithName("GetSystemOverview")
            .WithSummary("Gets the current MITMSpec platform overview.");

        var traffic = endpoints.MapGroup("/api/traffic").RequireRateLimiting("api").WithTags("Traffic");
        traffic.MapGet("/events", async ([FromServices] ITrafficQueryService service, int? take, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetRecentAsync(take ?? 25, cancellationToken)))
            .WithName("GetRecentTrafficEvents")
            .WithSummary("Gets recent traffic events for the operator dashboard.");

        traffic.MapGet("/events/{eventId}", async Task<IResult> (string eventId, [FromServices] ITrafficQueryService service, CancellationToken cancellationToken) =>
        {
            var detail = await service.GetByIdAsync(eventId, cancellationToken);
            return detail is null ? TypedResults.NotFound() : TypedResults.Ok(detail);
        })
        .WithName("GetTrafficEventById")
        .WithSummary("Gets a traffic event by event id.");

        var ingest = endpoints.MapGroup("/ingest/traffic").RequireRateLimiting("api").WithTags("Ingest");
        ingest.MapPost("/", async Task<IResult> (TrafficEnvelopeV1 envelope, [FromServices] ITrafficIngestService service, CancellationToken cancellationToken) =>
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

        var auth = endpoints.MapGroup("/api/auth").RequireRateLimiting("api").WithTags("Auth");
        auth.MapPost("/login-attempts", async Task<IResult> (LoginAttemptRequestDto request, [FromServices] IAuditService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.Username))
            {
                return TypedResults.BadRequest(CreateValidationProblem("actorId and username are required."));
            }

            await service.RecordLoginAttemptAsync(request, cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("RecordLoginAttempt")
        .WithSummary("Records a successful or failed login attempt in the audit log.");

        var users = endpoints.MapGroup("/api/users").RequireRateLimiting("api").WithTags("Users");
        users.MapGet("/", async ([FromServices] IUserQueryService service, int? take, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetRecentAsync(take ?? 50, cancellationToken)))
        .WithName("GetRecentUsers")
        .WithSummary("Gets recent users for operator workflows.");

        users.MapPost("/", async Task<IResult> (CreateUserRequestDto request, [FromServices] IUserLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.DisplayName))
            {
                return TypedResults.BadRequest(CreateValidationProblem("actorId, userId, and displayName are required."));
            }

            var user = await service.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/api/users/{user.UserId}", user);
        })
        .WithName("CreateUser")
        .WithSummary("Creates a user and records the action in the audit log.");

        users.MapPost("/{userId}/deactivate", async Task<IResult> (string userId, DeactivateUserRequestDto request, [FromServices] IUserLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.Reason))
            {
                return TypedResults.BadRequest(CreateValidationProblem("userId, actorId, and reason are required."));
            }

            var user = await service.DeactivateAsync(userId, request, cancellationToken);
            return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
        })
        .WithName("DeactivateUser")
        .WithSummary("Deactivates a user and records the action in the audit log.");

        var tokens = endpoints.MapGroup("/api/tokens").RequireRateLimiting("api").WithTags("Tokens");
        tokens.MapGet("/", async ([FromServices] ITokenQueryService service, int? take, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetRecentAsync(take ?? 50, cancellationToken)))
        .WithName("GetRecentTokens")
        .WithSummary("Gets recent provisioning tokens for operator workflows.");

        tokens.MapPost("/", async Task<IResult> (CreateTokenRequestDto request, [FromServices] ITokenLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Description))
            {
                return TypedResults.BadRequest(CreateValidationProblem("actorId, userId, and description are required."));
            }

            var token = await service.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/api/tokens/{token.Token.TokenId}", token);
        })
        .WithName("CreateToken")
        .WithSummary("Creates a provisioning token and records the action in the audit log.");

        tokens.MapPost("/{tokenId}/redeem", async Task<IResult> (string tokenId, RedeemTokenRequestDto request, [FromServices] ITokenLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(tokenId) || string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.PeerId) || string.IsNullOrWhiteSpace(request.RedeemSecret))
            {
                return TypedResults.BadRequest(CreateValidationProblem("tokenId, actorId, peerId, and redeemSecret are required."));
            }

            var token = await service.RedeemAsync(tokenId, request, cancellationToken);
            return token is null ? TypedResults.NotFound() : TypedResults.Ok(token);
        })
        .WithName("RedeemToken")
        .WithSummary("Marks a provisioning token as redeemed and records the action in the audit log.");

        tokens.MapPost("/{tokenId}/revoke", async Task<IResult> (string tokenId, TokenActionRequestDto request, [FromServices] ITokenLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(tokenId) || string.IsNullOrWhiteSpace(request.ActorId))
            {
                return TypedResults.BadRequest(CreateValidationProblem("tokenId and actorId are required."));
            }

            var token = await service.RevokeAsync(tokenId, request, cancellationToken);
            return token is null ? TypedResults.NotFound() : TypedResults.Ok(token);
        })
        .WithName("RevokeToken")
        .WithSummary("Revokes a provisioning token and records the action in the audit log.");

        var peers = endpoints.MapGroup("/api/peers").RequireRateLimiting("api").WithTags("Peers");
        peers.MapGet("/", async ([FromServices] IPeerQueryService service, int? take, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetRecentAsync(take ?? 50, cancellationToken)))
        .WithName("GetRecentPeers")
        .WithSummary("Gets recent peer bindings for operator workflows.");

        peers.MapPost("/", async Task<IResult> (BindPeerRequestDto request, [FromServices] IPeerLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.PeerId) || string.IsNullOrWhiteSpace(request.UserId))
            {
                return TypedResults.BadRequest(CreateValidationProblem("actorId, peerId, and userId are required."));
            }

            var peer = await service.BindPeerAsync(request, cancellationToken);
            return TypedResults.Created($"/api/peers/{peer.PeerId}", peer);
        })
        .WithName("BindPeer")
        .WithSummary("Binds a peer to a user and records the action in the audit log.");

        peers.MapPost("/{peerId}/remove", async Task<IResult> (string peerId, RemovePeerRequestDto request, [FromServices] IPeerLifecycleService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(peerId) || string.IsNullOrWhiteSpace(request.ActorId) || string.IsNullOrWhiteSpace(request.Reason))
            {
                return TypedResults.BadRequest(CreateValidationProblem("peerId, actorId, and reason are required."));
            }

            var peer = await service.RemovePeerAsync(peerId, request, cancellationToken);
            return peer is null ? TypedResults.NotFound() : TypedResults.Ok(peer);
        })
        .WithName("RemovePeer")
        .WithSummary("Removes a peer binding and records the action in the audit log.");

        endpoints.MapPlaceholderGroup("/api/gateways", "Gateway management endpoints are planned for a later phase.");
        endpoints.MapPlaceholderGroup("/api/webhooks", "Webhook endpoints are planned for a later phase.");

        var audit = endpoints.MapGroup("/api/audit").RequireRateLimiting("api").WithTags("Audit");
        audit.MapGet("/entries", async ([FromServices] IAuditService service, int? take, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetRecentAsync(take ?? 50, cancellationToken)))
            .WithName("GetRecentAuditEntries")
            .WithSummary("Gets recent audit entries for security-sensitive platform actions.");

        return endpoints;
    }

    private static ProblemDetails CreateValidationProblem(string detail) =>
        new()
        {
            Title = "Invalid request",
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        };

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
