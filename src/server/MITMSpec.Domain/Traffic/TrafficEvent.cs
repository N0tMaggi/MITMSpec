namespace MITMSpec.Domain.Traffic;

public sealed record TrafficEvent(
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
    string TraceId)
{
    public bool HasBodies => !string.IsNullOrWhiteSpace(RequestBody) || !string.IsNullOrWhiteSpace(ResponseBody);
}
