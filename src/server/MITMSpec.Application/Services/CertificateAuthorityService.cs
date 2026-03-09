using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using MITMSpec.Application.Abstractions;
using MITMSpec.Application.Configuration;
using MITMSpec.Contracts.Audit;
using MITMSpec.Contracts.Certificates;

namespace MITMSpec.Application.Services;

public sealed class CertificateAuthorityService(
    ICertificateAuthorityStore certificateAuthorityStore,
    IAuditEntryStore auditEntryStore,
    IDataProtectionProvider dataProtectionProvider,
    IOptions<ProvisioningOptions> provisioningOptions) : ICertificateAuthorityService
{
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("MITMSpec.CertificateAuthority.PrivateKey.v1");

    public async Task<CertificateAuthorityDto> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var current = await certificateAuthorityStore.GetActiveAsync(cancellationToken);
        if (current is not null)
        {
            return current;
        }

        var created = CreateCertificateAuthority();
        var saved = await certificateAuthorityStore.AddAsync(created.Authority, created.EncryptedPrivateKeyPem, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "ca.created",
                "certificate_authority",
                saved.CertificateAuthorityId,
                "system",
                "success",
                $"Initial certificate authority '{saved.CommonName}' was created."),
            cancellationToken);

        return saved;
    }

    public async Task<CertificateAuthorityDto> RotateAsync(RotateCertificateAuthorityRequestDto request, CancellationToken cancellationToken = default)
    {
        var current = await GetActiveAsync(cancellationToken);
        var created = CreateCertificateAuthority();
        var saved = await certificateAuthorityStore.AddAsync(created.Authority, created.EncryptedPrivateKeyPem, cancellationToken);
        await certificateAuthorityStore.DeactivateAsync(current.CertificateAuthorityId, DateTimeOffset.UtcNow, cancellationToken);

        await auditEntryStore.AddAsync(
            new AuditEntryDto(
                Guid.NewGuid().ToString("n"),
                DateTimeOffset.UtcNow,
                "ca.rotated",
                "certificate_authority",
                saved.CertificateAuthorityId,
                request.ActorId,
                "success",
                $"Certificate authority rotated. Previous authority '{current.CertificateAuthorityId}' replaced. Reason: {request.Reason}."),
            cancellationToken);

        return saved;
    }

    private CreatedCertificateAuthority CreateCertificateAuthority()
    {
        var now = DateTimeOffset.UtcNow;
        using var rsa = RSA.Create(4096);
        var subject = new X500DistinguishedName($"CN={provisioningOptions.Value.RootCaCommonName}");
        var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(
            X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature,
            true));
        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        using var certificate = request.CreateSelfSigned(now.AddMinutes(-5), now.AddYears(10));
        var certPem = certificate.ExportCertificatePem();
        var privateKeyPem = rsa.ExportPkcs8PrivateKeyPem();

        var authority = new CertificateAuthorityDto(
            $"ca_{Guid.NewGuid():N}",
            provisioningOptions.Value.RootCaCommonName,
            certificate.Thumbprint,
            certificate.SerialNumber,
            certPem,
            now,
            now,
            true,
            null);

        return new CreatedCertificateAuthority(authority, _dataProtector.Protect(privateKeyPem));
    }

    private sealed record CreatedCertificateAuthority(
        CertificateAuthorityDto Authority,
        string EncryptedPrivateKeyPem);
}
