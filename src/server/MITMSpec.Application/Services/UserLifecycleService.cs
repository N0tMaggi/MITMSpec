using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Users;

namespace MITMSpec.Application.Services;

public sealed class UserLifecycleService(IUserStore userStore, IAuditEntryStore auditEntryStore) : IUserLifecycleService
{
    public async Task<UserDto> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = new UserDto(request.UserId, request.DisplayName, true, DateTimeOffset.UtcNow, null);
        var saved = await userStore.UpsertAsync(user, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "user.created",
                "user",
                saved.UserId,
                request.ActorId,
                "success",
                $"User '{saved.DisplayName}' was created."),
            cancellationToken);

        return saved;
    }

    public async Task<UserDto?> DeactivateAsync(string userId, DeactivateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await userStore.GetByIdAsync(userId, cancellationToken);
        if (current is null)
        {
            return null;
        }

        var updated = await userStore.UpsertAsync(current with { IsActive = false, DeactivatedAtUtc = DateTimeOffset.UtcNow }, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "user.deactivated",
                "user",
                updated.UserId,
                request.ActorId,
                "success",
                $"User '{updated.DisplayName}' was deactivated. Reason: {request.Reason}."),
            cancellationToken);

        return updated;
    }
}
