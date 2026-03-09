namespace MITMSpec.Infrastructure.Persistence.Entities;

public sealed class TrafficEventEntity
{
    public string EventId { get; set; } = string.Empty;

    public DateTimeOffset ObservedAtUtc { get; set; }

    public string GatewayId { get; set; } = string.Empty;

    public string PeerId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Scheme { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public int? StatusCode { get; set; }

    public string MitmDisposition { get; set; } = string.Empty;

    public string? BypassReason { get; set; }

    public long? RequestBodyBytes { get; set; }

    public long? ResponseBodyBytes { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }

    public string TraceId { get; set; } = string.Empty;
}
