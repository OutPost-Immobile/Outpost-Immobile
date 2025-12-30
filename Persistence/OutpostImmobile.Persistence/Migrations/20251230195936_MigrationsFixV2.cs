using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrationsFixV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersInternal_Routes_RouteId",
                table: "UsersInternal");

            migrationBuilder.AlterColumn<long>(
                name: "RouteId",
                table: "UsersInternal",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInternal_Routes_RouteId",
                table: "UsersInternal",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersInternal_Routes_RouteId",
                table: "UsersInternal");

            migrationBuilder.AlterColumn<long>(
                name: "RouteId",
                table: "UsersInternal",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInternal_Routes_RouteId",
                table: "UsersInternal",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
