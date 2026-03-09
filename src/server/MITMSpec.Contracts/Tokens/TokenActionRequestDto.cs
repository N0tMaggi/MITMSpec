namespace MITMSpec.Contracts.Tokens;

public sealed record TokenActionRequestDto(
    string ActorId,
    string? Reason);
