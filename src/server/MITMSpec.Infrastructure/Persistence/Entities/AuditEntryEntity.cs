namespace MITMSpec.Infrastructure.Persistence.Entities;

public sealed class AuditEntryEntity
{
    public string AuditEntryId { get; set; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string SubjectType { get; set; } = string.Empty;

    public string SubjectId { get; set; } = string.Empty;

    public string ActorId { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;
}
