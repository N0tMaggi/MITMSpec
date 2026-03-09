using MITMSpec.Contracts.Peers;

namespace MITMSpec.Contracts.Tokens;

public sealed record TokenRedeemResultDto(
    TokenDto Token,
    PeerDto Peer);
