using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MITMSpec.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProvisioningSecretsAndPeerAttribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "tokens",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAtUtc",
                table: "tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RevocationReason",
                table: "tokens",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretHash",
                table: "tokens",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EnrollmentTokenId",
                table: "peers",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAtUtc",
                table: "tokens");

            migrationBuilder.DropColumn(
                name: "RevocationReason",
                table: "tokens");

            migrationBuilder.DropColumn(
                name: "SecretHash",
                table: "tokens");

            migrationBuilder.DropColumn(
                name: "EnrollmentTokenId",
                table: "peers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "tokens",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "Pending");
        }
    }
}
