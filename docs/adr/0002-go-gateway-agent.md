# ADR 0002: Gateway Host Agent In Go

## Status

Accepted

## Decision

The privileged gateway host agent is implemented in Go.

## Rationale

- good fit for a small privileged long-running service
- simpler Linux and Windows packaging than Python
- static binary distribution is operationally simpler
- host networking and service supervision concerns are isolated from the control plane
