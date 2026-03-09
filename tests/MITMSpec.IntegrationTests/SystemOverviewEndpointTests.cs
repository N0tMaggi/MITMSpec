using System.Net;
using System.Net.Http.Json;
using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.System;
using MITMSpec.Contracts.Traffic;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Contracts.Users;

namespace MITMSpec.IntegrationTests;

public class SystemOverviewEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public SystemOverviewEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetOverviewReturnsCurrentPlatformSnapshot()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/system/overview");
        var payload = await response.Content.ReadFromJsonAsync<SystemOverviewDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal("MITMSpec", payload.PlatformName);
        Assert.Contains("Go", payload.GatewayAgent);
    }

    [Fact]
    public async Task IngestThenQueryReturnsPersistedTrafficEvent()
    {
        using var client = _factory.CreateClient();
        var userId = "user-test";
        var peerId = "peer-test";

        await client.PostAsJsonAsync("/api/users", new CreateUserRequestDto("admin-001", userId, "User Test"));
        await client.PostAsJsonAsync("/api/peers", new BindPeerRequestDto("admin-001", peerId, userId));

        var envelope = new TrafficEnvelopeV1(
            "evt-int-001",
            DateTimeOffset.UtcNow,
            "gw-test",
            peerId,
            userId,
            "https",
            "GET",
            "example.test",
            "/ingest-check",
            200,
            "inspected",
            null,
            0,
            0,
            null,
            null,
            "trace-int-001");

        var ingestResponse = await client.PostAsJsonAsync("/ingest/traffic", envelope);
        var trafficResponse = await client.GetAsync("/api/traffic/events");
        var events = await trafficResponse.Content.ReadFromJsonAsync<List<TrafficEventSummaryDto>>();

        Assert.Equal(HttpStatusCode.OK, ingestResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, trafficResponse.StatusCode);
        Assert.NotNull(events);
        Assert.Contains(events, item => item.EventId == envelope.EventId);
    }

    [Fact]
    public async Task RedeemedTokenAllowsPeerAttributedIngestWithoutEnvelopeUserId()
    {
        using var client = _factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("n")[..8];
        var userId = $"user-{suffix}";
        var peerId = $"peer-{suffix}";

        await client.PostAsJsonAsync("/api/users", new CreateUserRequestDto("admin-001", userId, $"User {suffix}"));
        var createTokenResponse = await client.PostAsJsonAsync("/api/tokens", new CreateTokenRequestDto("admin-001", userId, "Enrollment token", 24));
        var issuedToken = await createTokenResponse.Content.ReadFromJsonAsync<IssuedTokenDto>();

        Assert.Equal(HttpStatusCode.Created, createTokenResponse.StatusCode);
        Assert.NotNull(issuedToken);

        var redeemResponse = await client.PostAsJsonAsync(
            $"/api/tokens/{issuedToken.Token.TokenId}/redeem",
            new RedeemTokenRequestDto("gateway-001", peerId, issuedToken.RedeemSecret, "client-public-key-001"));
        var redeemed = await redeemResponse.Content.ReadFromJsonAsync<TokenRedeemResultDto>();

        var envelope = new TrafficEnvelopeV1(
            $"evt-{suffix}",
            DateTimeOffset.UtcNow,
            "gw-test",
            peerId,
            null,
            "https",
            "GET",
            "example.test",
            "/redeemed-ingest",
            200,
            "inspected",
            null,
            0,
            0,
            null,
            null,
            $"trace-{suffix}");

        var ingestResponse = await client.PostAsJsonAsync("/ingest/traffic", envelope);
        var detailResponse = await client.GetAsync($"/api/traffic/events/{envelope.EventId}");
        var detail = await detailResponse.Content.ReadFromJsonAsync<TrafficEventDetailDto>();

        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);
        Assert.NotNull(redeemed);
        Assert.Equal(peerId, redeemed.Peer.PeerId);
        Assert.Equal("client-public-key-001", redeemed.Peer.ClientPublicKey);
        Assert.StartsWith("10.44.0.", redeemed.Peer.TunnelAddressCidr);
        Assert.Equal(redeemed.Peer.TunnelAddressCidr, redeemed.WireGuard.AssignedAddressCidr);
        Assert.Equal(HttpStatusCode.OK, ingestResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        Assert.NotNull(detail);
        Assert.Equal(userId, detail.UserId);
        Assert.Equal(peerId, detail.PeerId);
    }
}
