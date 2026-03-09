using MITMSpec.Contracts.Certificates;
using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Contracts.Enrollment;

public sealed record EnrollmentPackageDto(
    string PackageId,
    DateTimeOffset IssuedAtUtc,
    IssuedTokenDto IssuedToken,
    CertificateAuthorityDto CertificateAuthority,
    string ControlPlaneBaseUrl,
    string RedeemEndpointPath,
    string GatewayTunnelNetworkCidr,
    string WireGuardConfigTemplate,
    string[] Notes);
