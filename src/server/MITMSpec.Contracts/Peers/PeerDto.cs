namespace MITMSpec.Contracts.Peers;

public sealed record PeerDto(
    string PeerId,
    string UserId,
    string? EnrollmentTokenId,
    string? TunnelAddressCidr,
    string? ClientPublicKey,
    bool IsBound,
    DateTimeOffset BoundAtUtc,
    DateTimeOffset? RemovedAtUtc);
