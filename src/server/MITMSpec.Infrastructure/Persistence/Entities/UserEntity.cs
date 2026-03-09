namespace MITMSpec.Infrastructure.Persistence.Entities;

public sealed class UserEntity
{
    public string UserId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? DeactivatedAtUtc { get; set; }
}
