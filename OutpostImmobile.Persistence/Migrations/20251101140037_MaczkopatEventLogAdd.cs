using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MaczkopatEventLogAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaczkopatEventLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventLogType = table.Column<int>(type: "integer", nullable: false),
                    LogMessage = table.Column<string>(type: "text", nullable: true),
                    MaczkopatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaczkopatEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaczkopatEventLogs_Maczkopats_MaczkopatId",
                        column: x => x.MaczkopatId,
                        principalTable: "Maczkopats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaczkopatEventLogs_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaczkopatEventLogs_CreatedById",
                table: "MaczkopatEventLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaczkopatEventLogs_MaczkopatId",
                table: "MaczkopatEventLogs",
                column: "MaczkopatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaczkopatEventLogs");
        }
    }
}
