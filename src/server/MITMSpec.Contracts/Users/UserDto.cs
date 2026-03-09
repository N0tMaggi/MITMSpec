namespace MITMSpec.Contracts.Users;

public sealed record UserDto(
    string UserId,
    string DisplayName,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DeactivatedAtUtc);
