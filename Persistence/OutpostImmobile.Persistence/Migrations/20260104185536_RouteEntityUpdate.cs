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
                name: "StartAddressName",
                table: "Routes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                 UPDATE "Routes" r
                                 SET "EndAddressName" = CONCAT(ae."City", ' ', ae."Street", ' ', ae."BuildingNumber"),
                                     "StartAddressName" = CONCAT(asrt."City", ' ', asrt."Street", ' ', asrt."BuildingNumber")
                                 FROM "Addresses" asrt, "Addresses" ae
                                 WHERE r."StartAddressId" = asrt."Id" 
                                   AND r."EndAddressId" = ae."Id";
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
