using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cruzadas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Category = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    EventName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PropertiesJson = table.Column<string>(type: "text", nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_logs_EventName",
                table: "app_logs",
                column: "EventName");

            migrationBuilder.CreateIndex(
                name: "IX_app_logs_Level",
                table: "app_logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_app_logs_Timestamp",
                table: "app_logs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_logs");
        }
    }
}
