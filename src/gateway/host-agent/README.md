# MITMSpec Gateway Host Agent

The gateway host agent is implemented in Go and is responsible for the privileged host-side duties that do not belong inside the ASP.NET Core control plane:

- WireGuard peer lifecycle
- routing and firewall orchestration
- proxy path supervision
- health reporting
- future Windows service integration

## Current State

The host agent now performs the first real control-plane integration step:

- loads validated runtime configuration from environment variables
- polls the control plane for `GET /api/gateways/configuration/current`
- validates the returned desired state
- keeps the latest desired state in memory
- logs only snapshot changes and polling failures

It does not yet apply WireGuard peers, routing, or firewall state. That is the next implementation phase.

## Environment Variables

- `MITMSPEC_GATEWAY_NODE_ID`
  Required. Stable logical gateway identifier.
- `MITMSPEC_CONTROL_PLANE_BASE_URL`
  Required. Absolute base URL of the MITMSpec control plane, for example `https://control-plane.example.test`.
- `MITMSPEC_GATEWAY_PLATFORM`
  Optional. Defaults to `linux`.
- `MITMSPEC_WIREGUARD_INTERFACE`
  Optional. Defaults to `wg-mitmspec`.
- `MITMSPEC_GATEWAY_POLL_INTERVAL`
  Optional. Defaults to `15s`.
- `MITMSPEC_GATEWAY_REQUEST_TIMEOUT`
  Optional. Defaults to `10s`.
- `MITMSPEC_GATEWAY_EXIT_AFTER_BOOT`
  Optional. Set to `1` to fetch one snapshot and exit.

## Example

```powershell
$env:MITMSPEC_GATEWAY_NODE_ID = "gw-lab-01"
$env:MITMSPEC_CONTROL_PLANE_BASE_URL = "https://localhost:5001"
$env:MITMSPEC_GATEWAY_PLATFORM = "linux"
$env:MITMSPEC_GATEWAY_EXIT_AFTER_BOOT = "1"
& "C:\Program Files\Go\bin\go.exe" run .\cmd\mitmspec-gateway
```
