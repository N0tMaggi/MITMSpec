using MITMSpec.Contracts.System;

namespace MITMSpec.Application.Abstractions;

public interface ISystemOverviewService
{
    Task<SystemOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default);
}
