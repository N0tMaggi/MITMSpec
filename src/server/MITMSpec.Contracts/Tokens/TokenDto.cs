namespace MITMSpec.Contracts.Tokens;

public sealed record TokenDto(
    string TokenId,
    string UserId,
    string Status,
    string Description,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? RedeemedAtUtc,
    DateTimeOffset? RevokedAtUtc);
