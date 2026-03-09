using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MITMSpec.Contracts.System;

namespace MITMSpec.IntegrationTests;

public class SystemOverviewEndpointTests : IClassFixture<WebApplicationFactory<MITMSpec.App.Program>>
{
    private readonly WebApplicationFactory<MITMSpec.App.Program> _factory;

    public SystemOverviewEndpointTests(WebApplicationFactory<MITMSpec.App.Program> factory)
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
}
