using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LocationMarkerEntityChangeLongLatToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_Maczkopats_MaczkopatEntityId",
                table: "Parcels");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersExternal_UsersInternal_CreatedById",
                table: "UsersExternal");

            migrationBuilder.DropIndex(
                name: "IX_UsersExternal_CreatedById",
                table: "UsersExternal");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UsersExternal");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "UsersExternal");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UsersExternal");

            migrationBuilder.RenameColumn(
                name: "FriendlyId",
                table: "Maczkopats",
                newName: "Code");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaczkopatEntityId",
                table: "Parcels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "Parcels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Locations",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Locations",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_Maczkopats_MaczkopatEntityId",
                table: "Parcels",
                column: "MaczkopatEntityId",
                principalTable: "Maczkopats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_Maczkopats_MaczkopatEntityId",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "Product",
                table: "Parcels");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Maczkopats",
                newName: "FriendlyId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UsersExternal",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "UsersExternal",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UsersExternal",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MaczkopatEntityId",
                table: "Parcels",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Longitude",
                table: "Locations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Latitude",
                table: "Locations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateIndex(
                name: "IX_UsersExternal_CreatedById",
                table: "UsersExternal",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_Maczkopats_MaczkopatEntityId",
                table: "Parcels",
                column: "MaczkopatEntityId",
                principalTable: "Maczkopats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersExternal_UsersInternal_CreatedById",
                table: "UsersExternal",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");
        }
    }
}
