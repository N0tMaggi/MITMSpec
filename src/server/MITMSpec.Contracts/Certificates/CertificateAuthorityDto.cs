namespace MITMSpec.Contracts.Certificates;

public sealed record CertificateAuthorityDto(
    string CertificateAuthorityId,
    string CommonName,
    string Thumbprint,
    string SerialNumber,
    string CertificatePem,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ActivatedAtUtc,
    bool IsActive,
    DateTimeOffset? RotatedAtUtc);
