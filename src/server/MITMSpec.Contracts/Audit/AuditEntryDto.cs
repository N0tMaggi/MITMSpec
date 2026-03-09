namespace MITMSpec.Contracts.Audit;

public sealed record AuditEntryDto(
    string AuditEntryId,
    DateTimeOffset OccurredAtUtc,
    string ActionType,
    string SubjectType,
    string SubjectId,
    string ActorId,
    string Result,
    string Detail);
