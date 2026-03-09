using MITMSpec.Contracts.Users;

namespace MITMSpec.Application.Abstractions;

public interface IUserStore
{
    Task<UserDto> UpsertAsync(UserDto user, CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
