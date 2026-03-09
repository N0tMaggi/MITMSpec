using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Users;

namespace MITMSpec.Application.Services;

public sealed class UserQueryService(IUserStore userStore) : IUserQueryService
{
    public Task<IReadOnlyList<UserDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
        => userStore.GetRecentAsync(Math.Clamp(take, 1, 200), cancellationToken);
}
