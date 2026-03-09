using MITMSpec.Contracts.Users;

namespace MITMSpec.Application.Abstractions;

public interface IUserLifecycleService
{
    Task<UserDto> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);

    Task<UserDto?> DeactivateAsync(string userId, DeactivateUserRequestDto request, CancellationToken cancellationToken = default);
}
