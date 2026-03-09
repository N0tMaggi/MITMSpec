package runtime

import (
	"testing"
	"time"

	"github.com/selfmade/mitmspec/gateway/internal/contracts"
)

func TestBuildDesiredStateAcceptsValidSnapshot(t *testing.T) {
	t.Parallel()

	snapshot := contracts.GatewayConfigurationSnapshot{
		SnapshotID:                 "snap-001",
		GeneratedAtUTC:             time.Now().UTC(),
		GatewayEndpoint:            "vpn.example.test:51820",
		GatewayPublicKey:           "gateway-public-key",
		TunnelNetworkCIDR:          "10.44.0.0/24",
		DNSServer:                  "1.1.1.1",
		AllowedIPs:                 "0.0.0.0/0, ::/0",
		PersistentKeepaliveSeconds: 25,
		PeerAssignments: []contracts.GatewayPeerAssignment{
			{
				PeerID:            "peer-001",
				UserID:            "user-001",
				TunnelAddressCIDR: "10.44.0.10/24",
				ClientPublicKey:   "pubkey-001",
				BoundAtUTC:        time.Now().UTC(),
			},
		},
	}

	state, err := BuildDesiredState(snapshot)
	if err != nil {
		t.Fatalf("expected valid snapshot, got error: %v", err)
	}

	if len(state.ByPeerID) != 1 {
		t.Fatalf("expected 1 peer assignment, got %d", len(state.ByPeerID))
	}
}

func TestBuildDesiredStateRejectsDuplicateTunnelAddress(t *testing.T) {
	t.Parallel()

	snapshot := contracts.GatewayConfigurationSnapshot{
		SnapshotID:                 "snap-002",
		GeneratedAtUTC:             time.Now().UTC(),
		GatewayEndpoint:            "vpn.example.test:51820",
		GatewayPublicKey:           "gateway-public-key",
		TunnelNetworkCIDR:          "10.44.0.0/24",
		DNSServer:                  "1.1.1.1",
		AllowedIPs:                 "0.0.0.0/0, ::/0",
		PersistentKeepaliveSeconds: 25,
		PeerAssignments: []contracts.GatewayPeerAssignment{
			{
				PeerID:            "peer-001",
				UserID:            "user-001",
				TunnelAddressCIDR: "10.44.0.10/24",
				ClientPublicKey:   "pubkey-001",
				BoundAtUTC:        time.Now().UTC(),
			},
			{
				PeerID:            "peer-002",
				UserID:            "user-002",
				TunnelAddressCIDR: "10.44.0.10/24",
				ClientPublicKey:   "pubkey-002",
				BoundAtUTC:        time.Now().UTC(),
			},
		},
	}

	if _, err := BuildDesiredState(snapshot); err == nil {
		t.Fatal("expected duplicate tunnel address validation error")
	}
}
