package config

import (
	"fmt"
	"net/url"
	"os"
	"strings"
	"time"
)

type Settings struct {
	NodeID              string
	Platform            string
	WireGuardInterface  string
	ControlPlaneBaseURL string
	PollInterval        time.Duration
	RequestTimeout      time.Duration
	ExitAfterBoot       bool
}

func LoadFromEnv() (Settings, error) {
	settings := Settings{
		NodeID:              strings.TrimSpace(os.Getenv("MITMSPEC_GATEWAY_NODE_ID")),
		Platform:            envOrDefault("MITMSPEC_GATEWAY_PLATFORM", "linux"),
		WireGuardInterface:  envOrDefault("MITMSPEC_WIREGUARD_INTERFACE", "wg-mitmspec"),
		ControlPlaneBaseURL: strings.TrimRight(strings.TrimSpace(os.Getenv("MITMSPEC_CONTROL_PLANE_BASE_URL")), "/"),
		ExitAfterBoot:       os.Getenv("MITMSPEC_GATEWAY_EXIT_AFTER_BOOT") == "1",
	}

	pollInterval, err := durationFromEnv("MITMSPEC_GATEWAY_POLL_INTERVAL", 15*time.Second)
	if err != nil {
		return Settings{}, err
	}

	requestTimeout, err := durationFromEnv("MITMSPEC_GATEWAY_REQUEST_TIMEOUT", 10*time.Second)
	if err != nil {
		return Settings{}, err
	}

	settings.PollInterval = pollInterval
	settings.RequestTimeout = requestTimeout

	if err := settings.Validate(); err != nil {
		return Settings{}, err
	}

	return settings, nil
}

func (s Settings) Validate() error {
	if s.NodeID == "" {
		return fmt.Errorf("MITMSPEC_GATEWAY_NODE_ID is required")
	}

	if s.ControlPlaneBaseURL == "" {
		return fmt.Errorf("MITMSPEC_CONTROL_PLANE_BASE_URL is required")
	}

	parsedURL, err := url.Parse(s.ControlPlaneBaseURL)
	if err != nil || parsedURL.Scheme == "" || parsedURL.Host == "" {
		return fmt.Errorf("MITMSPEC_CONTROL_PLANE_BASE_URL must be a valid absolute URL")
	}

	if s.WireGuardInterface == "" {
		return fmt.Errorf("MITMSPEC_WIREGUARD_INTERFACE is required")
	}

	if s.PollInterval < time.Second {
		return fmt.Errorf("MITMSPEC_GATEWAY_POLL_INTERVAL must be at least 1s")
	}

	if s.RequestTimeout < time.Second {
		return fmt.Errorf("MITMSPEC_GATEWAY_REQUEST_TIMEOUT must be at least 1s")
	}

	return nil
}

func envOrDefault(key string, fallback string) string {
	value := strings.TrimSpace(os.Getenv(key))
	if value == "" {
		return fallback
	}

	return value
}

func durationFromEnv(key string, fallback time.Duration) (time.Duration, error) {
	value := strings.TrimSpace(os.Getenv(key))
	if value == "" {
		return fallback, nil
	}

	duration, err := time.ParseDuration(value)
	if err != nil {
		return 0, fmt.Errorf("%s must be a valid duration: %w", key, err)
	}

	return duration, nil
}
