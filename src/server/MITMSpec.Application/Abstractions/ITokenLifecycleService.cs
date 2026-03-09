using MITMSpec.Contracts.Tokens;

namespace MITMSpec.Application.Abstractions;

public interface ITokenLifecycleService
{
    Task<IssuedTokenDto> CreateAsync(CreateTokenRequestDto request, CancellationToken cancellationToken = default);

    Task<TokenRedeemResultDto?> RedeemAsync(string tokenId, RedeemTokenRequestDto request, CancellationToken cancellationToken = default);

    Task<TokenDto?> RevokeAsync(string tokenId, TokenActionRequestDto request, CancellationToken cancellationToken = default);
}
