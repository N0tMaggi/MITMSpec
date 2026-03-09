using MITMSpec.Contracts.Gateways;

namespace MITMSpec.Application.Abstractions;

public interface IGatewayConfigurationService
{
    Task<IReadOnlyList<GatewayPeerAssignmentDto>> GetPeerAssignmentsAsync(CancellationToken cancellationToken = default);

    Task<GatewayConfigurationSnapshotDto> GetCurrentSnapshotAsync(CancellationToken cancellationToken = default);
}
