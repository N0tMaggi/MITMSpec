namespace MITMSpec.Contracts.Tokens;

public sealed record IssuedTokenDto(
    TokenDto Token,
    string RedeemSecret);
