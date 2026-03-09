namespace MITMSpec.Contracts.Traffic;

public sealed record TrafficEventDetailDto(
    string EventId,
    DateTimeOffset ObservedAtUtc,
    string GatewayId,
    string PeerId,
    string UserId,
    string Scheme,
    string Method,
    string Host,
    string Path,
    int? StatusCode,
    string MitmDisposition,
    string? BypassReason,
    long? RequestBodyBytes,
    long? ResponseBodyBytes,
    string? RequestBody,
    string? ResponseBody,
    string TraceId);
