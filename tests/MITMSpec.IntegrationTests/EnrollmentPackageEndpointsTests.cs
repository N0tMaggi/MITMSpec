using System.Net;
using System.Net.Http.Json;
using MITMSpec.Contracts.Certificates;
using MITMSpec.Contracts.Enrollment;
using MITMSpec.Contracts.Users;

namespace MITMSpec.IntegrationTests;

public class EnrollmentPackageEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public EnrollmentPackageEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetActiveCertificateAuthorityCreatesAuthorityOnDemand()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/ca/active");
        var payload = await response.Content.ReadFromJsonAsync<CertificateAuthorityDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.IsActive);
        Assert.Contains("BEGIN CERTIFICATE", payload.CertificatePem);
    }

    [Fact]
    public async Task RotateCertificateAuthorityReplacesTheActiveAuthority()
    {
        using var client = _factory.CreateClient();

        var current = await client.GetFromJsonAsync<CertificateAuthorityDto>("/api/ca/active");
        var rotateResponse = await client.PostAsJsonAsync(
            "/api/ca/rotate",
            new RotateCertificateAuthorityRequestDto("admin-001", "scheduled rotation"));
        var rotated = await rotateResponse.Content.ReadFromJsonAsync<CertificateAuthorityDto>();
        var activeAfterRotation = await client.GetFromJsonAsync<CertificateAuthorityDto>("/api/ca/active");

        Assert.NotNull(current);
        Assert.Equal(HttpStatusCode.OK, rotateResponse.StatusCode);
        Assert.NotNull(rotated);
        Assert.NotNull(activeAfterRotation);
        Assert.NotEqual(current.CertificateAuthorityId, rotated.CertificateAuthorityId);
        Assert.Equal(rotated.CertificateAuthorityId, activeAfterRotation.CertificateAuthorityId);
        Assert.Contains("BEGIN CERTIFICATE", rotated.CertificatePem);
    }

    [Fact]
    public async Task CreateEnrollmentPackageReturnsTokenAndBootstrapMaterial()
    {
        using var client = _factory.CreateClient();
        var userId = $"user-{Guid.NewGuid():N}".Substring(0, 13);

        var createUserResponse = await client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequestDto("admin-001", userId, $"User {userId}"));

        var packageResponse = await client.PostAsJsonAsync(
            "/api/enrollment/packages",
            new CreateEnrollmentPackageRequestDto("admin-001", userId, "Primary workstation", 24));
        var package = await packageResponse.Content.ReadFromJsonAsync<EnrollmentPackageDto>();

        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, packageResponse.StatusCode);
        Assert.NotNull(package);
        Assert.NotEmpty(package.PackageId);
        Assert.NotEmpty(package.IssuedToken.Token.TokenId);
        Assert.NotEmpty(package.IssuedToken.RedeemSecret);
        Assert.Contains("BEGIN CERTIFICATE", package.CertificateAuthority.CertificatePem);
        Assert.Contains("[Interface]", package.WireGuardConfigTemplate);
        Assert.Contains(package.IssuedToken.Token.TokenId, package.RedeemEndpointPath);
        Assert.NotEmpty(package.Notes);
    }
}
