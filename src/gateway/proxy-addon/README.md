# MITMSpec Proxy Addon

This package contains the Python addon that will run inside `mitmproxy` and export normalized traffic envelopes to the MITMSpec ingest API.

The current baseline includes:

- package metadata
- addon bootstrap
- initial envelope normalization stub

It does not yet implement:

- authenticated ingest delivery
- retry and buffering
- policy-aware redaction
- failure quarantine
- TLS or pinning diagnostics enrichment
