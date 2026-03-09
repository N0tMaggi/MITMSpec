namespace MITMSpec.Contracts.Auth;

public sealed record LoginAttemptRequestDto(
    string ActorId,
    string Username,
    bool Succeeded,
    string? FailureReason);
