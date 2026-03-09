namespace MITMSpec.Contracts.Traffic;

public sealed record TrafficEventSummaryDto(
    string EventId,
    DateTimeOffset ObservedAtUtc,
    string GatewayId,
    string PeerId,
    string UserId,
    string Method,
    string Host,
    string Path,
    int? StatusCode,
    string MitmDisposition,
    bool HasBodies);
