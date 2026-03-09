using System.Net;
using System.Net.Http.Json;
using MITMSpec.Contracts.Gateways;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Contracts.Users;

namespace MITMSpec.IntegrationTests;

public class GatewayConfigurationEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public GatewayConfigurationEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CurrentGatewayConfigurationIncludesRedeemedPeerAssignments()
    {
        using var client = _factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("n")[..8];
        var userId = $"user-{suffix}";
        var peerId = $"peer-{suffix}";

        await client.PostAsJsonAsync("/api/users", new CreateUserRequestDto("admin-001", userId, $"User {suffix}"));
        var createTokenResponse = await client.PostAsJsonAsync("/api/tokens", new CreateTokenRequestDto("admin-001", userId, "Gateway peer", 24));
        var issuedToken = await createTokenResponse.Content.ReadFromJsonAsync<IssuedTokenDto>();

        Assert.NotNull(issuedToken);

        var redeemResponse = await client.PostAsJsonAsync(
            $"/api/tokens/{issuedToken.Token.TokenId}/redeem",
            new RedeemTokenRequestDto("gateway-001", peerId, issuedToken.RedeemSecret, "client-public-key-gateway"));

        var assignmentsResponse = await client.GetAsync("/api/gateways/peer-assignments");
        var assignments = await assignmentsResponse.Content.ReadFromJsonAsync<List<GatewayPeerAssignmentDto>>();
        var configurationResponse = await client.GetAsync("/api/gateways/configuration/current");
        var configuration = await configurationResponse.Content.ReadFromJsonAsync<GatewayConfigurationSnapshotDto>();

        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, assignmentsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, configurationResponse.StatusCode);
        Assert.NotNull(assignments);
        Assert.NotNull(configuration);
        Assert.NotEmpty(configuration.SnapshotId);
        Assert.Equal("vpn.example.test:51820", configuration.GatewayEndpoint);
        Assert.Equal("10.44.0.0/24", configuration.TunnelNetworkCidr);
        Assert.Contains(assignments, item => item.PeerId == peerId && item.ClientPublicKey == "client-public-key-gateway");
        Assert.Contains(configuration.PeerAssignments, item => item.PeerId == peerId && item.UserId == userId);
    }
}
