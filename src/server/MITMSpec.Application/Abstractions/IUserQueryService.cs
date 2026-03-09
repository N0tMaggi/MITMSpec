using MITMSpec.Contracts.Users;

namespace MITMSpec.Application.Abstractions;

public interface IUserQueryService
{
    Task<IReadOnlyList<UserDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
