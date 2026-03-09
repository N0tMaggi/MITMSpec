namespace MITMSpec.Contracts.Users;

public sealed record CreateUserRequestDto(
    string ActorId,
    string UserId,
    string DisplayName);
