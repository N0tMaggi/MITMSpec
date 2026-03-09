using System.Net;
using System.Net.Http.Json;
using MITMSpec.Contracts.System;
using MITMSpec.Contracts.Traffic;

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

        var envelope = new TrafficEnvelopeV1(
            "evt-int-001",
            DateTimeOffset.UtcNow,
            "gw-test",
            "peer-test",
            "user-test",
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
}
