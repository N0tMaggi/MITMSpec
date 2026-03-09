namespace MITMSpec.Contracts.Gateways;

public sealed record GatewayPeerAssignmentDto(
    string PeerId,
    string UserId,
    string TunnelAddressCidr,
    string ClientPublicKey,
    string? EnrollmentTokenId,
    DateTimeOffset BoundAtUtc);
