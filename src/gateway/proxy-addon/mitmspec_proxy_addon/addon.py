from __future__ import annotations

import json
import logging
import uuid
from datetime import datetime, timezone


LOGGER = logging.getLogger("mitmspec.proxy")


class MITMSpecAddon:
    def load(self, loader) -> None:
        LOGGER.info("MITMSpec proxy addon loaded")

    def response(self, flow) -> None:
        request = flow.request
        response = flow.response

        envelope = {
            "eventId": str(uuid.uuid4()),
            "observedAtUtc": datetime.now(timezone.utc).isoformat(),
            "gatewayId": "gw-lab-01",
            "peerId": "peer-placeholder",
            "userId": "user-placeholder",
            "scheme": request.scheme,
            "method": request.method,
            "host": request.pretty_host,
            "path": request.path,
            "statusCode": response.status_code,
            "mitmDisposition": "inspected",
            "bypassReason": None,
            "requestBodyBytes": len(request.raw_content or b""),
            "responseBodyBytes": len(response.raw_content or b""),
            "requestBody": None,
            "responseBody": None,
            "traceId": flow.id,
        }

        LOGGER.info("normalized envelope: %s", json.dumps(envelope))


addons = [MITMSpecAddon()]
