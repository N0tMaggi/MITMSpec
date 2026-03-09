package config

import "os"

type Settings struct {
	NodeID            string
	Platform          string
	WireGuardInterface string
}

func LoadFromEnv() Settings {
	return Settings{
		NodeID:            os.Getenv("MITMSPEC_GATEWAY_NODE_ID"),
		Platform:          os.Getenv("MITMSPEC_GATEWAY_PLATFORM"),
		WireGuardInterface: envOrDefault("MITMSPEC_WIREGUARD_INTERFACE", "wg0"),
	}
}

func envOrDefault(key string, fallback string) string {
	value := os.Getenv(key)
	if value == "" {
		return fallback
	}

	return value
}
