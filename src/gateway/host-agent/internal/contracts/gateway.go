package contracts

import "time"

type GatewayPeerAssignment struct {
	PeerID            string    `json:"peerId"`
	UserID            string    `json:"userId"`
	TunnelAddressCIDR string    `json:"tunnelAddressCidr"`
	ClientPublicKey   string    `json:"clientPublicKey"`
	EnrollmentTokenID *string   `json:"enrollmentTokenId"`
	BoundAtUTC        time.Time `json:"boundAtUtc"`
}

type GatewayConfigurationSnapshot struct {
	SnapshotID                 string                  `json:"snapshotId"`
	GeneratedAtUTC             time.Time               `json:"generatedAtUtc"`
	GatewayEndpoint            string                  `json:"gatewayEndpoint"`
	GatewayPublicKey           string                  `json:"gatewayPublicKey"`
	TunnelNetworkCIDR          string                  `json:"tunnelNetworkCidr"`
	DNSServer                  string                  `json:"dnsServer"`
	AllowedIPs                 string                  `json:"allowedIps"`
	PersistentKeepaliveSeconds int                     `json:"persistentKeepaliveSeconds"`
	PeerAssignments            []GatewayPeerAssignment `json:"peerAssignments"`
}
