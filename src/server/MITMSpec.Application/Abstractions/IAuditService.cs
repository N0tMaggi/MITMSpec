using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Auth;
using MITMSpec.Contracts.Peers;
using MITMSpec.Contracts.Tokens;
using MITMSpec.Contracts.Users;

namespace MITMSpec.Application.Abstractions;

public interface IAuditService
{
    Task RecordLoginAttemptAsync(LoginAttemptRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditEntryDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
