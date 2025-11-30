using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgrouting", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Alias = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    BuildingNumber = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<Point>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticEnums",
                columns: table => new
                {
                    EnumName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticEnums", x => x.EnumName);
                });

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

            migrationBuilder.CreateTable(
                name: "StaticEnumTranslations",
                columns: table => new
                {
                    EnumValue = table.Column<int>(type: "integer", nullable: false),
                    EnumName = table.Column<string>(type: "text", nullable: false),
                    TranslationLanguage = table.Column<string>(type: "text", nullable: false),
                    Translation = table.Column<string>(type: "text", nullable: false),
                    EnumEntityEnumName = table.Column<string>(type: "text", nullable: true),
                    StaticEnumEntityEnumName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticEnumTranslations", x => new { x.EnumValue, x.EnumName, x.TranslationLanguage });
                    table.ForeignKey(
                        name: "FK_StaticEnumTranslations_StaticEnums_EnumEntityEnumName",
                        column: x => x.EnumEntityEnumName,
                        principalTable: "StaticEnums",
                        principalColumn: "EnumName");
                    table.ForeignKey(
                        name: "FK_StaticEnumTranslations_StaticEnums_StaticEnumEntityEnumName",
                        column: x => x.StaticEnumEntityEnumName,
                        principalTable: "StaticEnums",
                        principalColumn: "EnumName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersInternal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInternal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersInternal_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Maczkopats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    AreaId = table.Column<long>(type: "bigint", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maczkopats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maczkopats_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Maczkopats_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Maczkopats_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartAddressId = table.Column<long>(type: "bigint", nullable: false),
                    EndAddressId = table.Column<long>(type: "bigint", nullable: false),
                    Distace = table.Column<long>(type: "bigint", nullable: false),
                    Locations = table.Column<Point[]>(type: "geometry[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UsersExternal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    InternalUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserInternalId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersExternal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersExternal_UsersInternal_UserInternalId",
                        column: x => x.UserInternalId,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    DistanceRidden = table.Column<long>(type: "bigint", nullable: false),
                    RouteEntityId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Routes_RouteEntityId",
                        column: x => x.RouteEntityId,
                        principalTable: "Routes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parcels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyId = table.Column<string>(type: "text", nullable: false),
                    Product = table.Column<string>(type: "text", nullable: false),
                    FromUserExternalId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceiverUserExternalId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaczkopatEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserExternalId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcels_Maczkopats_MaczkopatEntityId",
                        column: x => x.MaczkopatEntityId,
                        principalTable: "Maczkopats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parcels_UsersExternal_UserExternalId",
                        column: x => x.UserExternalId,
                        principalTable: "UsersExternal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parcels_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NumberTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateNumber = table.Column<string>(type: "text", nullable: false),
                    VehicleEntityId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumberTemplates_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NumberTemplates_Vehicles_VehicleEntityId",
                        column: x => x.VehicleEntityId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunicationEventLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sender = table.Column<string>(type: "text", nullable: true),
                    Receiver = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ParcelId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationEventLogs_Parcels_ParcelId",
                        column: x => x.ParcelId,
                        principalTable: "Parcels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunicationEventLogs_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ParcelEventLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ParcelEventLogType = table.Column<int>(type: "integer", nullable: false),
                    ParcelId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcelEventLogs_Parcels_ParcelId",
                        column: x => x.ParcelId,
                        principalTable: "Parcels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParcelEventLogs_UsersInternal_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UsersInternal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationEventLogs_CreatedById",
                table: "CommunicationEventLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationEventLogs_ParcelId",
                table: "CommunicationEventLogs",
                column: "ParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_MaczkopatEventLogs_CreatedById",
                table: "MaczkopatEventLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaczkopatEventLogs_MaczkopatId",
                table: "MaczkopatEventLogs",
                column: "MaczkopatId");

            migrationBuilder.CreateIndex(
                name: "IX_Maczkopats_AddressId",
                table: "Maczkopats",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Maczkopats_AreaId",
                table: "Maczkopats",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Maczkopats_CreatedById",
                table: "Maczkopats",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NumberTemplates_CreatedById",
                table: "NumberTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NumberTemplates_VehicleEntityId",
                table: "NumberTemplates",
                column: "VehicleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcelEventLogs_CreatedById",
                table: "ParcelEventLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ParcelEventLogs_ParcelId",
                table: "ParcelEventLogs",
                column: "ParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_CreatedById",
                table: "Parcels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_MaczkopatEntityId",
                table: "Parcels",
                column: "MaczkopatEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_UserExternalId",
                table: "Parcels",
                column: "UserExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_CreatedById",
                table: "Routes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StaticEnumTranslations_EnumEntityEnumName",
                table: "StaticEnumTranslations",
                column: "EnumEntityEnumName");

            migrationBuilder.CreateIndex(
                name: "IX_StaticEnumTranslations_StaticEnumEntityEnumName",
                table: "StaticEnumTranslations",
                column: "StaticEnumEntityEnumName");

            migrationBuilder.CreateIndex(
                name: "IX_UsersExternal_UserInternalId",
                table: "UsersExternal",
                column: "UserInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInternal_RoleId",
                table: "UsersInternal",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CreatedById",
                table: "Vehicles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RouteEntityId",
                table: "Vehicles",
                column: "RouteEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunicationEventLogs");

            migrationBuilder.DropTable(
                name: "MaczkopatEventLogs");

            migrationBuilder.DropTable(
                name: "NumberTemplates");

            migrationBuilder.DropTable(
                name: "ParcelEventLogs");

            migrationBuilder.DropTable(
                name: "StaticEnumTranslations");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Parcels");

            migrationBuilder.DropTable(
                name: "StaticEnums");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Maczkopats");

            migrationBuilder.DropTable(
                name: "UsersExternal");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "UsersInternal");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
