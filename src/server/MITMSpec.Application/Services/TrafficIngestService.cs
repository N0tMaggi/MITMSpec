using MITMSpec.Application.Abstractions;
using MITMSpec.Contracts.Traffic;
using MITMSpec.Domain.Traffic;

namespace MITMSpec.Application.Services;

public sealed class TrafficIngestService(ITrafficEventStore trafficEventStore) : ITrafficIngestService
{
    public async Task<TrafficIngestResponseDto> IngestAsync(TrafficEnvelopeV1 envelope, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(envelope.EventId) ||
            string.IsNullOrWhiteSpace(envelope.GatewayId) ||
            string.IsNullOrWhiteSpace(envelope.PeerId) ||
            string.IsNullOrWhiteSpace(envelope.Method) ||
            string.IsNullOrWhiteSpace(envelope.Host) ||
            string.IsNullOrWhiteSpace(envelope.Path) ||
            string.IsNullOrWhiteSpace(envelope.TraceId))
        {
            return new TrafficIngestResponseDto(envelope.EventId, TrafficIngestOutcome.Quarantined, "Envelope validation failed.");
        }

        if (string.IsNullOrWhiteSpace(envelope.UserId))
        {
            return new TrafficIngestResponseDto(envelope.EventId, TrafficIngestOutcome.Quarantined, "Envelope could not be attributed to a user.");
        }

        var trafficEvent = new TrafficEvent(
            envelope.EventId,
            envelope.ObservedAtUtc,
            envelope.GatewayId,
            envelope.PeerId,
            envelope.UserId,
            envelope.Scheme,
            envelope.Method.ToUpperInvariant(),
            envelope.Host,
            envelope.Path,
            envelope.StatusCode,
            envelope.MitmDisposition,
            envelope.BypassReason,
            envelope.RequestBodyBytes,
            envelope.ResponseBodyBytes,
            envelope.RequestBody,
            envelope.ResponseBody,
            envelope.TraceId);

        var result = await trafficEventStore.AddAsync(trafficEvent, cancellationToken);

        return result switch
        {
            TrafficStoreWriteResult.Added => new TrafficIngestResponseDto(envelope.EventId, TrafficIngestOutcome.Accepted, "Envelope accepted."),
            _ => new TrafficIngestResponseDto(envelope.EventId, TrafficIngestOutcome.Duplicate, "Envelope already exists.")
        };
    }
}
