using MITMSpec.Contracts.Certificates;

namespace MITMSpec.Application.Abstractions;

public interface ICertificateAuthorityService
{
    Task<CertificateAuthorityDto> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<CertificateAuthorityDto> RotateAsync(RotateCertificateAuthorityRequestDto request, CancellationToken cancellationToken = default);
}
