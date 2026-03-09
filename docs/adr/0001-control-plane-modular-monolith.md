# ADR 0001: Control Plane As A Modular Monolith

## Status

Accepted

## Decision

The control plane is implemented as a modular monolith on ASP.NET Core 10 with a Blazor Web App UI, Minimal APIs, and a separate worker host for background processing.

## Rationale

- fastest path from greenfield to operable product
- simpler consistency across auth, audit, and traffic processing
- lower self-hosted operational overhead than early microservices
