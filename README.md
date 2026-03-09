# MITMSpec

MITMSpec is a self-hosted traffic inspection platform that routes client traffic through WireGuard to a central gateway, inspects supported HTTPS traffic through a MITM proxy, attributes events to the correct user, and exposes that traffic through an operator-focused web UI and webhook delivery pipeline.

## Core Capabilities

- Multi-user traffic inspection with strict per-user attribution
- WireGuard-based client connectivity for Linux and Windows clients
- Central gateway routing and transparent proxy supervision
- HTTPS inspection through a trusted private certificate authority
- Metadata and full-body traffic capture with retention and redaction controls
- Operator web UI for traffic exploration, provisioning, health, and audit workflows
- Extensible event delivery to external services through signed webhooks
- Linux-first production deployment with a planned Windows Server gateway path

## Architecture Overview

MITMSpec is split into a control plane and a data plane:

- Control plane: `ASP.NET Core 10` modular monolith
- Admin UI: `Blazor Web App`
- HTTP surface: `Minimal APIs`
- Background processing: ASP.NET Core worker services with durable outbox patterns
- Primary storage: `PostgreSQL 16+`
- Gateway host agent: `Go`
- MITM integration: `Python` addon built around `mitmproxy`
- Deployment: `Docker Compose` for the control plane, host-level services for gateway duties

High-level traffic flow:

1. A client connects through WireGuard.
2. The gateway host agent applies routing and policy.
3. Supported traffic is forwarded into the MITM proxy path.
4. The proxy addon emits normalized traffic envelopes.
5. The control plane ingests, attributes, stores, and exposes the traffic.
6. Webhooks can forward selected events to external systems.

## Current Status

The repository currently contains the initial implementation baseline for the agreed roadmap:

- .NET 10 server solution skeleton
- Blazor control-plane app
- Worker project
- Domain, application, contracts, and infrastructure libraries
- Linux-first deployment scaffolding
- Go gateway source stub
- Python proxy addon source stub
- Architecture, ADR, and runbook placeholders

Planned delivery order:

1. Definition freeze and internal contracts
2. Linux lab proof of path
3. Control-plane foundation
4. Provisioning, peer identity, and CA workflows
5. Linux gateway MVP
6. Traffic pipeline, storage, full-body capture, and explorer
7. Webhooks, recovery, and hardening
8. Windows Server gateway support with MSI and EXE installers

## Technology Stack

- `ASP.NET Core 10`
- `Blazor Web App`
- `Minimal APIs`
- `PostgreSQL 16+`
- `Docker Compose`
- `Go` for the privileged gateway service
- `Python 3` + `mitmproxy` for proxy-side integration
- `OpenTelemetry` for tracing and metrics hooks
- `WiX Toolset v4` planned for Windows installer packaging

## Supported Environments

### Server

- Linux
- Windows Server

### Client

- Windows
- Linux

### Deployment

- Docker
- Docker Compose

Linux is the first production target for the full traffic path. Windows Server support is planned as a later delivery phase with native gateway packaging.

## Repository Structure

```text
.
|-- .private/                     Internal planning and engineering contract
|-- deploy/                       Compose files and deployment assets
|-- docs/                         Architecture notes, ADRs, and runbooks
|-- src/
|   |-- client/                   Future bootstrap tooling
|   |-- gateway/
|   |   |-- host-agent/           Go gateway host agent
|   |   `-- proxy-addon/          Python mitmproxy addon
|   `-- server/
|       |-- MITMSpec.App/         Blazor UI + Minimal APIs
|       |-- MITMSpec.Application/ Application layer
|       |-- MITMSpec.Contracts/   API and ingest contracts
|       |-- MITMSpec.Domain/      Domain types and invariants
|       |-- MITMSpec.Infrastructure/
|       `-- MITMSpec.Worker/      Background workers
|-- tests/                        Unit and integration tests
`-- MITMSpec.slnx                 .NET solution
```

## Development Prerequisites

- `.NET SDK 10.0.103` or newer in the .NET 10 line
- `Docker Desktop` or compatible Docker Engine + Compose
- `Python 3.10+`
- `Go 1.24+` for gateway builds
- `mitmproxy` for proxy integration work
- PostgreSQL client tooling is optional but useful for local diagnostics

## Local Development Workflow

### Restore and build the server solution

```powershell
dotnet restore MITMSpec.slnx
dotnet build MITMSpec.slnx
```

### Run the control-plane app

```powershell
dotnet run --project .\src\server\MITMSpec.App\MITMSpec.App.csproj
```

### Run the worker

```powershell
dotnet run --project .\src\server\MITMSpec.Worker\MITMSpec.Worker.csproj
```

### Start infrastructure dependencies

```powershell
docker compose -f .\deploy\compose\docker-compose.yml up -d
```

### Gateway and proxy development

- Gateway source lives in [`src/gateway/host-agent`](./src/gateway/host-agent)
- Proxy addon source lives in [`src/gateway/proxy-addon`](./src/gateway/proxy-addon)

The current repository includes source scaffolding for both components. Building the Go gateway requires a local Go toolchain.

## Deployment Model

The control plane is intended to run in containers:

- `mitmspec-app`
- `mitmspec-worker`
- `postgres`
- `mitmproxy`
- optional observability services

The gateway responsibilities stay on the host because they require privileged access to:

- WireGuard configuration
- routes and firewall rules
- proxy path supervision
- local host diagnostics

### Windows installer path

Windows Server support is planned with native installer artifacts:

- `MITMSpecGateway.msi`
- `MITMSpecGatewaySetup.exe`
- `MITMSpecBootstrap.exe`

These are part of the roadmap and are not implemented in this baseline yet.

## Security And Legal-Use Notice

MITMSpec is designed for environments where the operator is authorized to route, decrypt, inspect, and store traffic. It must only be used where the operator has a lawful basis and explicit authority to intercept that traffic.

The platform is intended to fail closed where attribution, routing, or trust state is uncertain. Secrets, keys, and sensitive payload handling must remain aligned with the engineering rules in `.private/AGENTS.md`.

## Important Limitations

- QUIC and HTTP/3 cannot be MITM-inspected the same way as HTTPS over TCP
- Certificate-pinned applications may bypass or fail inspection
- Clients must trust the MITMSpec CA for HTTPS inspection to succeed
- Full-body capture must be controlled through strict retention, redaction, and storage policies
- Windows Server support is planned, but Linux is the first-class production target

## Documentation Index

- [`docs/architecture/overview.md`](./docs/architecture/overview.md)
- [`docs/adr/0001-control-plane-modular-monolith.md`](./docs/adr/0001-control-plane-modular-monolith.md)
- [`docs/adr/0002-go-gateway-agent.md`](./docs/adr/0002-go-gateway-agent.md)
- [`docs/adr/0003-mitmproxy-python-addon.md`](./docs/adr/0003-mitmproxy-python-addon.md)
- [`docs/adr/0004-traffic-envelope-v1.md`](./docs/adr/0004-traffic-envelope-v1.md)
- [`docs/runbooks/linux-gateway-setup.md`](./docs/runbooks/linux-gateway-setup.md)
- [`docs/runbooks/windows-gateway-install.md`](./docs/runbooks/windows-gateway-install.md)

## Contribution And Quality Expectations

- Prefer framework conventions and production-grade defaults
- Add tests for success and failure paths
- Do not introduce silent failure handling or cross-user data shortcuts
- Keep control-plane API failures on `ProblemDetails`
- Keep the gateway and proxy integrations behind stable internal contracts

## License

MITMSpec is licensed under the MIT License. See [`LICENSE`](./LICENSE).
