using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Auth;

namespace MITMSpec.Application.Services;

public sealed class AuditService(IAuditEntryStore auditEntryStore) : IAuditService
{
    public Task<IReadOnlyList<AuditEntryDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
        => auditEntryStore.GetRecentAsync(Math.Clamp(take, 1, 200), cancellationToken);

    public Task RecordLoginAttemptAsync(LoginAttemptRequestDto request, CancellationToken cancellationToken = default)
    {
        var detail = request.Succeeded
            ? $"Login succeeded for username '{request.Username}'."
            : $"Login failed for username '{request.Username}'. Reason: {request.FailureReason ?? "unspecified"}.";

        return auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                request.Succeeded ? "login.succeeded" : "login.failed",
                "auth",
                request.Username,
                request.ActorId,
                request.Succeeded ? "success" : "failure",
                detail),
            cancellationToken);
    }
}
