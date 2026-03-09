# ADR 0003: MITM Integration Through Python And mitmproxy

## Status

Accepted

## Decision

Proxy-side traffic extraction is implemented as a Python addon for `mitmproxy`.

## Rationale

- `mitmproxy` is Python-native
- addon development is direct and low-friction
- proxy details stay isolated behind a stable ingest contract
