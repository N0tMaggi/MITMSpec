package controlplane

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"strings"
	"time"

	"github.com/selfmade/mitmspec/gateway/internal/contracts"
)

type Client struct {
	baseURL    string
	httpClient *http.Client
}

func NewClient(baseURL string, timeout time.Duration) *Client {
	return &Client{
		baseURL: strings.TrimRight(baseURL, "/"),
		httpClient: &http.Client{
			Timeout: timeout,
		},
	}
}

func (c *Client) GetCurrentGatewayConfiguration(ctx context.Context) (contracts.GatewayConfigurationSnapshot, error) {
	request, err := http.NewRequestWithContext(ctx, http.MethodGet, c.baseURL+"/api/gateways/configuration/current", nil)
	if err != nil {
		return contracts.GatewayConfigurationSnapshot{}, fmt.Errorf("create request: %w", err)
	}

	response, err := c.httpClient.Do(request)
	if err != nil {
		return contracts.GatewayConfigurationSnapshot{}, fmt.Errorf("execute request: %w", err)
	}
	defer response.Body.Close()

	if response.StatusCode != http.StatusOK {
		return contracts.GatewayConfigurationSnapshot{}, fmt.Errorf("unexpected status code: %d", response.StatusCode)
	}

	var snapshot contracts.GatewayConfigurationSnapshot
	if err := json.NewDecoder(response.Body).Decode(&snapshot); err != nil {
		return contracts.GatewayConfigurationSnapshot{}, fmt.Errorf("decode response: %w", err)
	}

	return snapshot, nil
}
