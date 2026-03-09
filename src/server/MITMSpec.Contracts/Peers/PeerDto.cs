namespace MITMSpec.Contracts.Peers;

public sealed record PeerDto(
    string PeerId,
    string UserId,
    bool IsBound,
    DateTimeOffset BoundAtUtc,
    DateTimeOffset? RemovedAtUtc);
