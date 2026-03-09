namespace MITMSpec.Contracts.Tokens;

public sealed record CreateTokenRequestDto(
    string ActorId,
    string UserId,
    string Description,
    int LifetimeHours);
