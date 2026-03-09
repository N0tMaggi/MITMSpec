using MITMSpec.Contracts.Certificates;

namespace MITMSpec.Application.Abstractions;

public interface ICertificateAuthorityStore
{
    Task<CertificateAuthorityDto?> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<CertificateAuthorityDto> AddAsync(
        CertificateAuthorityDto authority,
        string encryptedPrivateKeyPem,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(string certificateAuthorityId, DateTimeOffset rotatedAtUtc, CancellationToken cancellationToken = default);
}
