namespace MITMSpec.Contracts.Traffic;

public sealed record TrafficIngestResponseDto(
    string EventId,
    TrafficIngestOutcome Outcome,
    string Message);
