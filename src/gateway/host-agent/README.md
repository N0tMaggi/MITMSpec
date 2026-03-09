# MITMSpec Gateway Host Agent

The gateway host agent is implemented in Go and is responsible for the privileged host-side duties that do not belong inside the ASP.NET Core control plane:

- WireGuard peer lifecycle
- routing and firewall orchestration
- proxy path supervision
- health reporting
- future Windows service integration

The current repository state contains a bootstrap module and configuration stub. The full Linux-first gateway implementation is still ahead in the roadmap.
