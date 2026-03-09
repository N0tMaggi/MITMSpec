using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Abstractions;

public interface ITokenLifecycleService
{
    Task<TokenDto> CreateAsync(CreateTokenRequestDto request, CancellationToken cancellationToken = default);

    Task<TokenDto?> RedeemAsync(string tokenId, TokenActionRequestDto request, CancellationToken cancellationToken = default);

    Task<TokenDto?> RevokeAsync(string tokenId, TokenActionRequestDto request, CancellationToken cancellationToken = default);
}
