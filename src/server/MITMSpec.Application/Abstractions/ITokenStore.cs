using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Abstractions;

public interface ITokenStore
{
    Task<TokenDto> CreateAsync(TokenDto token, string secretHash, CancellationToken cancellationToken = default);

    Task<TokenDto> UpsertAsync(TokenDto token, CancellationToken cancellationToken = default);

    Task<TokenDto?> GetByIdAsync(string tokenId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TokenDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<string?> GetSecretHashAsync(string tokenId, CancellationToken cancellationToken = default);
}
