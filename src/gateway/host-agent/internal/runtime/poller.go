package runtime

import (
	"context"
	"fmt"
	"log"
	"time"

	"github.com/selfmade/mitmspec/gateway/internal/config"
	"github.com/selfmade/mitmspec/gateway/internal/controlplane"
)

type Poller struct {
	settings config.Settings
	client   *controlplane.Client
	store    *Store
	logger   *log.Logger
}

func NewPoller(settings config.Settings, client *controlplane.Client, store *Store, logger *log.Logger) *Poller {
	return &Poller{
		settings: settings,
		client:   client,
		store:    store,
		logger:   logger,
	}
}

func (p *Poller) Run(ctx context.Context) error {
	if err := p.pollOnce(ctx); err != nil {
		return err
	}

	if p.settings.ExitAfterBoot {
		return nil
	}

	ticker := time.NewTicker(p.settings.PollInterval)
	defer ticker.Stop()

	for {
		select {
		case <-ctx.Done():
			return nil
		case <-ticker.C:
			if err := p.pollOnce(ctx); err != nil {
				p.logger.Printf("gateway config poll failed: %v", err)
			}
		}
	}
}

func (p *Poller) pollOnce(ctx context.Context) error {
	operationContext, cancel := context.WithTimeout(ctx, p.settings.RequestTimeout)
	defer cancel()

	snapshot, err := p.client.GetCurrentGatewayConfiguration(operationContext)
	if err != nil {
		return fmt.Errorf("fetch gateway configuration: %w", err)
	}

	state, err := BuildDesiredState(snapshot)
	if err != nil {
		return fmt.Errorf("validate desired state: %w", err)
	}

	if p.store.Replace(state) {
		p.logger.Printf(
			"gateway desired state updated: snapshot=%s peers=%d endpoint=%s interface=%s",
			state.Snapshot.SnapshotID,
			len(state.Snapshot.PeerAssignments),
			state.Snapshot.GatewayEndpoint,
			p.settings.WireGuardInterface,
		)
	}

	return nil
}
