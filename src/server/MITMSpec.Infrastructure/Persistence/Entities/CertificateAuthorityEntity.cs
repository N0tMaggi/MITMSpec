namespace MITMSpec.Infrastructure.Persistence.Entities;

public sealed class CertificateAuthorityEntity
{
    public string CertificateAuthorityId { get; set; } = string.Empty;

    public string CommonName { get; set; } = string.Empty;

    public string Thumbprint { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string CertificatePem { get; set; } = string.Empty;

    public string EncryptedPrivateKeyPem { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset ActivatedAtUtc { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? RotatedAtUtc { get; set; }
}
