using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MITMSpec.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPeerWireGuardAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientPublicKey",
                table: "peers",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TunnelAddressCidr",
                table: "peers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_peers_TunnelAddressCidr",
                table: "peers",
                column: "TunnelAddressCidr",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_peers_TunnelAddressCidr",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "ClientPublicKey",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "TunnelAddressCidr",
                table: "peers");
        }
    }
}
