namespace MITMSpec.Contracts.Enrollment;

public sealed record CreateEnrollmentPackageRequestDto(
    string ActorId,
    string UserId,
    string Description,
    int LifetimeHours);
