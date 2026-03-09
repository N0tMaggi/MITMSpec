namespace MITMSpec.Contracts.Certificates;

public sealed record RotateCertificateAuthorityRequestDto(
    string ActorId,
    string Reason);
