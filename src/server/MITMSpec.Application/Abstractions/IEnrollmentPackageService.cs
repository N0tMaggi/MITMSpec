using MITMSpec.Contracts.Enrollment;

namespace MITMSpec.Application.Abstractions;

public interface IEnrollmentPackageService
{
    Task<EnrollmentPackageDto> CreateAsync(CreateEnrollmentPackageRequestDto request, CancellationToken cancellationToken = default);
}
