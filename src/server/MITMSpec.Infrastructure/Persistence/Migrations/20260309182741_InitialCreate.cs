using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MITMSpec.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "traffic_events",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ObservedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GatewayId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PeerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Scheme = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Host = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    MitmDisposition = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BypassReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RequestBodyBytes = table.Column<long>(type: "bigint", nullable: true),
                    ResponseBodyBytes = table.Column<long>(type: "bigint", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traffic_events", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_traffic_events_GatewayId_PeerId",
                table: "traffic_events",
                columns: new[] { "GatewayId", "PeerId" });

            migrationBuilder.CreateIndex(
                name: "IX_traffic_events_ObservedAtUtc",
                table: "traffic_events",
                column: "ObservedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_traffic_events_UserId",
                table: "traffic_events",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "traffic_events");
        }
    }
}
