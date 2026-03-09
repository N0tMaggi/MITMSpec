using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Services;

public sealed class TokenQueryService(ITokenStore tokenStore) : ITokenQueryService
{
    public Task<IReadOnlyList<TokenDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
        => tokenStore.GetRecentAsync(Math.Clamp(take, 1, 200), cancellationToken);
}
