using System.Net;
using Microsoft.Extensions.Options;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Configuration;

namespace MITMSpec.Application.Services;

public sealed class PeerAddressAllocator(
    IPeerStore peerStore,
    IOptions<ProvisioningOptions> provisioningOptions) : IPeerAddressAllocator
{
    public async Task<string> AllocateAsync(CancellationToken cancellationToken = default)
    {
        var network = provisioningOptions.Value.GatewayTunnelNetworkCidr;
        if (!TryParseIpv4Cidr(network, out var networkAddress, out var prefixLength))
        {
            throw new InvalidOperationException($"Provisioning network '{network}' is not a supported IPv4 CIDR.");
        }

        var usedAddresses = await peerStore.GetAllocatedTunnelAddressesAsync(cancellationToken);
        var used = usedAddresses
            .Select(ParseAssignedAddress)
            .Where(address => address is not null)
            .Select(address => address!.Value)
            .ToHashSet();

        var totalHosts = 1u << (32 - prefixLength);
        if (totalHosts <= 4)
        {
            throw new InvalidOperationException($"Provisioning network '{network}' does not have enough client addresses.");
        }

        var baseAddress = ToUint(networkAddress);
        var firstClient = baseAddress + 10;
        var lastClient = baseAddress + totalHosts - 2;

        for (var current = firstClient; current <= lastClient; current++)
        {
            if (used.Contains(current))
            {
                continue;
            }

            return $"{FromUint(current)}/{prefixLength}";
        }

        throw new InvalidOperationException($"Provisioning network '{network}' has no free client addresses remaining.");
    }

    private static uint? ParseAssignedAddress(string? cidr)
    {
        if (string.IsNullOrWhiteSpace(cidr))
        {
            return null;
        }

        var parts = cidr.Split('/', 2, StringSplitOptions.TrimEntries);
        return IPAddress.TryParse(parts[0], out var address) ? ToUint(address) : null;
    }

    private static bool TryParseIpv4Cidr(string cidr, out IPAddress address, out int prefixLength)
    {
        address = IPAddress.None;
        prefixLength = 0;

        var parts = cidr.Split('/', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || !int.TryParse(parts[1], out prefixLength) || prefixLength is < 0 or > 32)
        {
            return false;
        }

        if (!IPAddress.TryParse(parts[0], out var parsedAddress) ||
            parsedAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
        {
            return false;
        }

        address = parsedAddress;
        return true;
    }

    private static uint ToUint(IPAddress address)
    {
        var bytes = address.GetAddressBytes();
        return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
    }

    private static string FromUint(uint value)
        => string.Join(
            ".",
            (value >> 24) & 0xFF,
            (value >> 16) & 0xFF,
            (value >> 8) & 0xFF,
            value & 0xFF);
}
