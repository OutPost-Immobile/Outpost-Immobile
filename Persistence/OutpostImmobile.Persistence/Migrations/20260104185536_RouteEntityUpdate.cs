using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RouteEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Distace",
                table: "Routes",
                newName: "Distance");

            migrationBuilder.AddColumn<string>(
                name: "EndAddressName",
                table: "Routes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StarAddressName",
                table: "Routes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                 UPDATE r
                                 SET r.EndAddressName = updateEnd.EndAddressName,
                                     r.StartAddressName = updateStart.StartAddressName
                                 From "Routes" r
                                 JOIN (
                                     SELECT a."Id", a."Location", CONCAT(a."City", ' ', a."Street", ' ', a."BuildingNumber") AS StartAddressName
                                     FROM "Addresses" a
                                     WHERE a."Id" = r."StartAddressId"
                                 ) AS updateStart ON r."StartAddressId" = updateStart."Id"
                                 JOIN (
                                     SELECT a."Id", a."Location", CONCAT(a."City", ' ', a."Street", ' ', a."BuildingNumber") AS EndAddressName
                                     FROM "Addresses" a
                                     WHERE a."Id" = r."EndAddressId"
                                 ) AS updateEnd ON r."StartAddressId" = updateStart."Id"
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAddressName",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "StarAddressName",
                table: "Routes");

            migrationBuilder.RenameColumn(
                name: "Distance",
                table: "Routes",
                newName: "Distace");
        }
    }
}
