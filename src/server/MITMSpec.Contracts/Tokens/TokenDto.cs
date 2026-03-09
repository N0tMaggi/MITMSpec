namespace MITMSpec.Contracts.Tokens;

public sealed record TokenDto(
    string TokenId,
    string UserId,
    TokenStatus Status,
    string Description,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? RedeemedAtUtc,
    DateTimeOffset? RevokedAtUtc,
    string? RevocationReason);
