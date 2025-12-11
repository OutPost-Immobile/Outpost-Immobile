using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrationsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogMessage",
                table: "MaczkopatEventLogs",
                newName: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ParcelEventLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "CommunicationEventLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "MaczkopatEventLogs",
                newName: "LogMessage");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ParcelEventLogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "CommunicationEventLogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
