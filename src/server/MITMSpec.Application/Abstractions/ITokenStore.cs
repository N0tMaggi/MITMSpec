using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Abstractions;

public interface ITokenStore
{
    Task<TokenDto> UpsertAsync(TokenDto token, CancellationToken cancellationToken = default);

    Task<TokenDto?> GetByIdAsync(string tokenId, CancellationToken cancellationToken = default);
}
