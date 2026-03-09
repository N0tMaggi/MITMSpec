namespace MITMSpec.Infrastructure.Persistence.Entities;

public sealed class PeerEntity
{
    public string PeerId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string? EnrollmentTokenId { get; set; }

    public string? TunnelAddressCidr { get; set; }

    public string? ClientPublicKey { get; set; }

    public bool IsBound { get; set; }

    public DateTimeOffset BoundAtUtc { get; set; }

    public DateTimeOffset? RemovedAtUtc { get; set; }
}
