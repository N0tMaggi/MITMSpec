package main

import (
	"context"
	"log"
	"os/signal"
	"syscall"

	"github.com/selfmade/mitmspec/gateway/internal/config"
	"github.com/selfmade/mitmspec/gateway/internal/controlplane"
	"github.com/selfmade/mitmspec/gateway/internal/runtime"
)

func main() {
	logger := log.New(log.Writer(), "mitmspec-gateway: ", log.LstdFlags|log.Lmsgprefix)

	cfg, err := config.LoadFromEnv()
	if err != nil {
		logger.Fatalf("configuration error: %v", err)
	}

	logger.Printf(
		"gateway bootstrap: node=%s platform=%s interface=%s control-plane=%s poll=%s",
		cfg.NodeID,
		cfg.Platform,
		cfg.WireGuardInterface,
		cfg.ControlPlaneBaseURL,
		cfg.PollInterval,
	)

	client := controlplane.NewClient(cfg.ControlPlaneBaseURL, cfg.RequestTimeout)
	store := &runtime.Store{}
	poller := runtime.NewPoller(cfg, client, store, logger)

	ctx, cancel := signal.NotifyContext(context.Background(), syscall.SIGINT, syscall.SIGTERM)
	defer cancel()

	if err := poller.Run(ctx); err != nil {
		logger.Fatalf("gateway runtime failed: %v", err)
	}
}
