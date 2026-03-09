namespace MITMSpec.Contracts.Users;

public sealed record DeactivateUserRequestDto(
    string ActorId,
    string Reason);
