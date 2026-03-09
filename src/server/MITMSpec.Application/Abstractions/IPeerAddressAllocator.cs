namespace MITMSpec.Application.Abstractions;

public interface IPeerAddressAllocator
{
    Task<string> AllocateAsync(CancellationToken cancellationToken = default);
}
