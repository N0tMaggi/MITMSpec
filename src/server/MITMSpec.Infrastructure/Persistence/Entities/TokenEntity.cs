namespace MITMSpec.Infrastructure.Persistence.Entities;

public sealed class TokenEntity
{
    public string TokenId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? RedeemedAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }
}
