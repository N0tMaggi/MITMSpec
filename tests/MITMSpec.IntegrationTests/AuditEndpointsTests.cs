using System.Net;
using System.Net.Http.Json;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Auth;
using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Contracts.Users;

namespace MITMSpec.IntegrationTests;

public class AuditEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public AuditEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LifecycleActionsAreRecordedInAuditLog()
    {
        using var client = _factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("n")[..8];
        var userId = $"user-{suffix}";
        var peerId = $"peer-{suffix}";

        var createUserResponse = await client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequestDto("admin-001", userId, $"User {suffix}"));

        var deactivateUserResponse = await client.PostAsJsonAsync(
            $"/api/users/{userId}/deactivate",
            new DeactivateUserRequestDto("admin-001", "operator request"));

        var loginSuccessResponse = await client.PostAsJsonAsync(
            "/api/auth/login-attempts",
            new LoginAttemptRequestDto("system-auth", "operator@example.test", true, null));

        var loginFailureResponse = await client.PostAsJsonAsync(
            "/api/auth/login-attempts",
            new LoginAttemptRequestDto("system-auth", "operator@example.test", false, "invalid password"));

        var createRedeemTokenResponse = await client.PostAsJsonAsync(
            "/api/tokens",
            new CreateTokenRequestDto("admin-001", userId, "Redeem token", 24));
        var redeemToken = await createRedeemTokenResponse.Content.ReadFromJsonAsync<IssuedTokenDto>();

        var redeemTokenResponse = await client.PostAsJsonAsync(
            $"/api/tokens/{redeemToken!.Token.TokenId}/redeem",
            new RedeemTokenRequestDto("gateway-001", peerId, redeemToken.RedeemSecret));

        var createRevokeTokenResponse = await client.PostAsJsonAsync(
            "/api/tokens",
            new CreateTokenRequestDto("admin-001", userId, "Revoke token", 24));
        var revokeToken = await createRevokeTokenResponse.Content.ReadFromJsonAsync<IssuedTokenDto>();

        var revokeTokenResponse = await client.PostAsJsonAsync(
            $"/api/tokens/{revokeToken!.Token.TokenId}/revoke",
            new TokenActionRequestDto("admin-001", "operator revoked token"));

        var removePeerResponse = await client.PostAsJsonAsync(
            $"/api/peers/{peerId}/remove",
            new RemovePeerRequestDto("admin-001", "peer rotated"));

        var usersResponse = await client.GetAsync("/api/users?take=10");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var tokensResponse = await client.GetAsync("/api/tokens?take=10");
        var tokens = await tokensResponse.Content.ReadFromJsonAsync<List<TokenDto>>();
        var peersResponse = await client.GetAsync("/api/peers?take=10");
        var peers = await peersResponse.Content.ReadFromJsonAsync<List<PeerDto>>();
        var auditResponse = await client.GetAsync("/api/audit/entries?take=20");
        var auditEntries = await auditResponse.Content.ReadFromJsonAsync<List<AuditEntryDto>>();

        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, deactivateUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, loginSuccessResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, loginFailureResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createRedeemTokenResponse.StatusCode);
        Assert.NotNull(redeemToken);
        Assert.Equal(HttpStatusCode.OK, redeemTokenResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createRevokeTokenResponse.StatusCode);
        Assert.NotNull(revokeToken);
        Assert.Equal(HttpStatusCode.OK, revokeTokenResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, removePeerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, tokensResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, peersResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, auditResponse.StatusCode);

        Assert.NotNull(users);
        Assert.NotNull(tokens);
        Assert.NotNull(peers);
        Assert.NotNull(auditEntries);
        Assert.Contains(users, entry => entry.UserId == userId);
        Assert.Contains(tokens, entry => entry.TokenId == redeemToken.Token.TokenId);
        Assert.Contains(peers, entry => entry.PeerId == peerId);
        Assert.Contains(auditEntries, entry => entry.ActionType == "login.succeeded");
        Assert.Contains(auditEntries, entry => entry.ActionType == "login.failed");
        Assert.Contains(auditEntries, entry => entry.ActionType == "user.created");
        Assert.Contains(auditEntries, entry => entry.ActionType == "user.deactivated");
        Assert.Contains(auditEntries, entry => entry.ActionType == "token.created" && entry.SubjectId == redeemToken.Token.TokenId);
        Assert.Contains(auditEntries, entry => entry.ActionType == "token.redeemed" && entry.SubjectId == redeemToken.Token.TokenId);
        Assert.Contains(auditEntries, entry => entry.ActionType == "token.created" && entry.SubjectId == revokeToken.Token.TokenId);
        Assert.Contains(auditEntries, entry => entry.ActionType == "token.revoked" && entry.SubjectId == revokeToken.Token.TokenId);
        Assert.Contains(auditEntries, entry => entry.ActionType == "peer.bound" && entry.SubjectId == peerId);
        Assert.Contains(auditEntries, entry => entry.ActionType == "peer.removed" && entry.SubjectId == peerId);
    }
}
