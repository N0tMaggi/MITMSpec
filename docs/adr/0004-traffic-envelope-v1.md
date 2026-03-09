# ADR 0004: TrafficEnvelope v1 As The Internal Ingest Contract

## Status

Accepted

## Decision

All gateway and proxy traffic is submitted to the control plane through a versioned `TrafficEnvelope v1` contract.

## Rationale

- decouples proxy-specific formats from storage and UI
- makes testing and replay easier
- gives future gateway implementations a stable integration target
