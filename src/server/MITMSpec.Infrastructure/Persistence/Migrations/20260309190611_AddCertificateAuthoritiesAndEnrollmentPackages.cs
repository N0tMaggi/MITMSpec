using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MITMSpec.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateAuthoritiesAndEnrollmentPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificate_authorities",
                columns: table => new
                {
                    CertificateAuthorityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CommonName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Thumbprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CertificatePem = table.Column<string>(type: "text", nullable: false),
                    EncryptedPrivateKeyPem = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RotatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificate_authorities", x => x.CertificateAuthorityId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_certificate_authorities_IsActive",
                table: "certificate_authorities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_certificate_authorities_Thumbprint",
                table: "certificate_authorities",
                column: "Thumbprint",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificate_authorities");
        }
    }
}
