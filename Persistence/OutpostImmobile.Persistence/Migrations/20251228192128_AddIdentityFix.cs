using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunicationEventLogs_UsersInternal_CreatedById",
                table: "CommunicationEventLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaczkopatEventLogs_UsersInternal_CreatedById",
                table: "MaczkopatEventLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Maczkopats_UsersInternal_CreatedById",
                table: "Maczkopats");

            migrationBuilder.DropForeignKey(
                name: "FK_NumberTemplates_UsersInternal_CreatedById",
                table: "NumberTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ParcelEventLogs_UsersInternal_CreatedById",
                table: "ParcelEventLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_UsersInternal_CreatedById",
                table: "Parcels");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_UsersInternal_CreatedById",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersExternal_UsersInternal_UserInternalId",
                table: "UsersExternal");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInternal_Routes_RouteId",
                table: "UsersInternal");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInternal_UserRoles_RoleId",
                table: "UsersInternal");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_UsersInternal_CreatedById",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersInternal",
                table: "UsersInternal");

            migrationBuilder.DropIndex(
                name: "IX_UsersInternal_RoleId",
                table: "UsersInternal");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UsersInternal");

            migrationBuilder.RenameTable(
                name: "UsersInternal",
                newName: "AspNetUsers");

            migrationBuilder.RenameIndex(
                name: "IX_UsersInternal_RouteId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_RouteId");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Routes_RouteId",
                table: "AspNetUsers",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunicationEventLogs_AspNetUsers_CreatedById",
                table: "CommunicationEventLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaczkopatEventLogs_AspNetUsers_CreatedById",
                table: "MaczkopatEventLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maczkopats_AspNetUsers_CreatedById",
                table: "Maczkopats",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NumberTemplates_AspNetUsers_CreatedById",
                table: "NumberTemplates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParcelEventLogs_AspNetUsers_CreatedById",
                table: "ParcelEventLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_AspNetUsers_CreatedById",
                table: "Parcels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_AspNetUsers_CreatedById",
                table: "Routes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersExternal_AspNetUsers_UserInternalId",
                table: "UsersExternal",
                column: "UserInternalId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_AspNetUsers_CreatedById",
                table: "Vehicles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Routes_RouteId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunicationEventLogs_AspNetUsers_CreatedById",
                table: "CommunicationEventLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaczkopatEventLogs_AspNetUsers_CreatedById",
                table: "MaczkopatEventLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Maczkopats_AspNetUsers_CreatedById",
                table: "Maczkopats");

            migrationBuilder.DropForeignKey(
                name: "FK_NumberTemplates_AspNetUsers_CreatedById",
                table: "NumberTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ParcelEventLogs_AspNetUsers_CreatedById",
                table: "ParcelEventLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_AspNetUsers_CreatedById",
                table: "Parcels");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_AspNetUsers_CreatedById",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersExternal_AspNetUsers_UserInternalId",
                table: "UsersExternal");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_AspNetUsers_CreatedById",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "UsersInternal");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_RouteId",
                table: "UsersInternal",
                newName: "IX_UsersInternal_RouteId");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UsersInternal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "UsersInternal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "UsersInternal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UsersInternal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "UsersInternal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersInternal",
                table: "UsersInternal",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersInternal_RoleId",
                table: "UsersInternal",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunicationEventLogs_UsersInternal_CreatedById",
                table: "CommunicationEventLogs",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaczkopatEventLogs_UsersInternal_CreatedById",
                table: "MaczkopatEventLogs",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maczkopats_UsersInternal_CreatedById",
                table: "Maczkopats",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NumberTemplates_UsersInternal_CreatedById",
                table: "NumberTemplates",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParcelEventLogs_UsersInternal_CreatedById",
                table: "ParcelEventLogs",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_UsersInternal_CreatedById",
                table: "Parcels",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_UsersInternal_CreatedById",
                table: "Routes",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersExternal_UsersInternal_UserInternalId",
                table: "UsersExternal",
                column: "UserInternalId",
                principalTable: "UsersInternal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInternal_Routes_RouteId",
                table: "UsersInternal",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInternal_UserRoles_RoleId",
                table: "UsersInternal",
                column: "RoleId",
                principalTable: "UserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_UsersInternal_CreatedById",
                table: "Vehicles",
                column: "CreatedById",
                principalTable: "UsersInternal",
                principalColumn: "Id");
        }
    }
}
