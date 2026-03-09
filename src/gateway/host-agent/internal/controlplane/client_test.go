package controlplane

import (
	"context"
	"net/http"
	"net/http/httptest"
	"testing"
	"time"
)

func TestGetCurrentGatewayConfiguration(t *testing.T) {
	t.Parallel()

	server := httptest.NewServer(http.HandlerFunc(func(writer http.ResponseWriter, request *http.Request) {
		if request.URL.Path != "/api/gateways/configuration/current" {
			http.NotFound(writer, request)
			return
		}

		writer.Header().Set("Content-Type", "application/json")
		_, _ = writer.Write([]byte(`{
			"snapshotId":"snap-001",
			"generatedAtUtc":"2026-03-09T10:00:00Z",
			"gatewayEndpoint":"vpn.example.test:51820",
			"gatewayPublicKey":"gateway-public-key",
			"tunnelNetworkCidr":"10.44.0.0/24",
			"dnsServer":"1.1.1.1",
			"allowedIps":"0.0.0.0/0, ::/0",
			"persistentKeepaliveSeconds":25,
			"peerAssignments":[]
		}`))
	}))
	defer server.Close()

	client := NewClient(server.URL, 5*time.Second)
	snapshot, err := client.GetCurrentGatewayConfiguration(context.Background())
	if err != nil {
		t.Fatalf("expected successful configuration fetch, got error: %v", err)
	}

	if snapshot.SnapshotID != "snap-001" {
		t.Fatalf("unexpected snapshot id: %s", snapshot.SnapshotID)
	}

	if snapshot.GatewayEndpoint != "vpn.example.test:51820" {
		t.Fatalf("unexpected gateway endpoint: %s", snapshot.GatewayEndpoint)
	}
}
