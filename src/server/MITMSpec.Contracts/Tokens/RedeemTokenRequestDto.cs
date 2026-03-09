namespace MITMSpec.Contracts.Tokens;

public sealed record RedeemTokenRequestDto(
    string ActorId,
    string PeerId,
    string RedeemSecret,
    string ClientPublicKey);
