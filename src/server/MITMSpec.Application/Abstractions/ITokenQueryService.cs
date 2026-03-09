using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Abstractions;

public interface ITokenQueryService
{
    Task<IReadOnlyList<TokenDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
