using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.Enrollment;

namespace MITMSpec.Contracts.Tokens;

public sealed record TokenRedeemResultDto(
    TokenDto Token,
    PeerDto Peer,
    WireGuardPeerConfigurationDto WireGuard);
