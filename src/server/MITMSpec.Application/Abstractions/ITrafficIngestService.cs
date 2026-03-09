using MITMSpec.Contracts.Traffic;

namespace MITMSpec.Application.Abstractions;

public interface ITrafficIngestService
{
    Task<TrafficIngestResponseDto> IngestAsync(TrafficEnvelopeV1 envelope, CancellationToken cancellationToken = default);
}
