package main

import (
	"log"
	"os"

	"github.com/selfmade/mitmspec/gateway/internal/config"
)

func main() {
	cfg := config.LoadFromEnv()

	log.Printf(
		"mitmspec gateway bootstrap: node=%s platform=%s wireguard=%s",
		cfg.NodeID,
		cfg.Platform,
		cfg.WireGuardInterface,
	)

	if cfg.NodeID == "" {
		log.Println("warning: gateway node id is empty; runtime validation is not implemented yet")
	}

	if os.Getenv("MITMSPEC_GATEWAY_EXIT_AFTER_BOOT") == "1" {
		return
	}

	select {}
}
