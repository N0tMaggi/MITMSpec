using MITMSpec.Contracts.Audit;

namespace MITMSpec.Application.Abstractions;

public interface IAuditEntryStore
{
    Task AddAsync(AuditEntryDto entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditEntryDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
