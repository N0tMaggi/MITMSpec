# MITMSpec Architecture Overview

MITMSpec consists of a containerized control plane and a host-level gateway data plane.

## Control Plane

- `MITMSpec.App`: Blazor Web App and Minimal APIs
- `MITMSpec.Worker`: background processing host
- `MITMSpec.Application`: application services and use cases
- `MITMSpec.Domain`: core domain types and invariants
- `MITMSpec.Contracts`: public and internal contracts
- `MITMSpec.Infrastructure`: infrastructure implementations

## Data Plane

- Go host agent for WireGuard, routing, firewall orchestration, and health
- Python `mitmproxy` addon for normalized traffic export

## Primary Design Constraints

- fail closed on uncertain attribution
- strict per-user isolation
- observable by default
- Linux-first gateway support
- Windows gateway support later through native service packaging
