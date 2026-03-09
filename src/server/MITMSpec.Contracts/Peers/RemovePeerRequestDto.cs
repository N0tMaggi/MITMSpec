namespace MITMSpec.Contracts.Peers;

public sealed record RemovePeerRequestDto(
    string ActorId,
    string Reason);
