package runtime

import (
	"fmt"
	"net/netip"
	"sync"

	"github.com/selfmade/mitmspec/gateway/internal/contracts"
)

type DesiredState struct {
	Snapshot contracts.GatewayConfigurationSnapshot
	ByPeerID map[string]contracts.GatewayPeerAssignment
}

func BuildDesiredState(snapshot contracts.GatewayConfigurationSnapshot) (DesiredState, error) {
	if snapshot.SnapshotID == "" {
		return DesiredState{}, fmt.Errorf("snapshot id is required")
	}

	if snapshot.GatewayEndpoint == "" {
		return DesiredState{}, fmt.Errorf("gateway endpoint is required")
	}

	if snapshot.GatewayPublicKey == "" {
		return DesiredState{}, fmt.Errorf("gateway public key is required")
	}

	if snapshot.DNSServer == "" {
		return DesiredState{}, fmt.Errorf("dns server is required")
	}

	tunnelNetwork, err := netip.ParsePrefix(snapshot.TunnelNetworkCIDR)
	if err != nil {
		return DesiredState{}, fmt.Errorf("parse tunnel network: %w", err)
	}

	if !tunnelNetwork.Addr().Is4() {
		return DesiredState{}, fmt.Errorf("tunnel network must be IPv4")
	}

	byPeerID := make(map[string]contracts.GatewayPeerAssignment, len(snapshot.PeerAssignments))
	usedAddresses := make(map[netip.Addr]string, len(snapshot.PeerAssignments))
	usedPublicKeys := make(map[string]string, len(snapshot.PeerAssignments))

	for _, assignment := range snapshot.PeerAssignments {
		if assignment.PeerID == "" {
			return DesiredState{}, fmt.Errorf("peer assignment has empty peer id")
		}

		if assignment.UserID == "" {
			return DesiredState{}, fmt.Errorf("peer %s has empty user id", assignment.PeerID)
		}

		if assignment.ClientPublicKey == "" {
			return DesiredState{}, fmt.Errorf("peer %s has empty client public key", assignment.PeerID)
		}

		if _, exists := byPeerID[assignment.PeerID]; exists {
			return DesiredState{}, fmt.Errorf("duplicate peer id %s", assignment.PeerID)
		}

		assignmentPrefix, err := netip.ParsePrefix(assignment.TunnelAddressCIDR)
		if err != nil {
			return DesiredState{}, fmt.Errorf("peer %s has invalid tunnel address: %w", assignment.PeerID, err)
		}

		if !tunnelNetwork.Contains(assignmentPrefix.Addr()) {
			return DesiredState{}, fmt.Errorf("peer %s address %s is outside tunnel network %s", assignment.PeerID, assignment.TunnelAddressCIDR, snapshot.TunnelNetworkCIDR)
		}

		if existingPeerID, exists := usedAddresses[assignmentPrefix.Addr()]; exists {
			return DesiredState{}, fmt.Errorf("duplicate tunnel address %s for peers %s and %s", assignment.TunnelAddressCIDR, existingPeerID, assignment.PeerID)
		}

		if existingPeerID, exists := usedPublicKeys[assignment.ClientPublicKey]; exists {
			return DesiredState{}, fmt.Errorf("duplicate client public key for peers %s and %s", existingPeerID, assignment.PeerID)
		}

		usedAddresses[assignmentPrefix.Addr()] = assignment.PeerID
		usedPublicKeys[assignment.ClientPublicKey] = assignment.PeerID
		byPeerID[assignment.PeerID] = assignment
	}

	return DesiredState{
		Snapshot: snapshot,
		ByPeerID: byPeerID,
	}, nil
}

type Store struct {
	mu      sync.RWMutex
	current *DesiredState
}

func (s *Store) Replace(next DesiredState) (changed bool) {
	s.mu.Lock()
	defer s.mu.Unlock()

	if s.current != nil && s.current.Snapshot.SnapshotID == next.Snapshot.SnapshotID {
		return false
	}

	copy := next
	s.current = &copy
	return true
}

func (s *Store) Current() (DesiredState, bool) {
	s.mu.RLock()
	defer s.mu.RUnlock()

	if s.current == nil {
		return DesiredState{}, false
	}

	return *s.current, true
}
