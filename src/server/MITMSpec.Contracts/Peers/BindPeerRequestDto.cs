namespace MITMSpec.Contracts.Peers;

public sealed record BindPeerRequestDto(
    string ActorId,
    string PeerId,
    string UserId);
