namespace MITMSpec.Contracts.Peers;

public sealed record PeerDto(
    string PeerId,
    string UserId,
    string? EnrollmentTokenId,
    bool IsBound,
    DateTimeOffset BoundAtUtc,
    DateTimeOffset? RemovedAtUtc);
