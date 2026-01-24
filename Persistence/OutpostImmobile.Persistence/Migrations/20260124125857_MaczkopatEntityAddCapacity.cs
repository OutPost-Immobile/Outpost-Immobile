using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutpostImmobile.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MaczkopatEntityAddCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Capacity",
                table: "Maczkopats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
            
            // Random number from 10 to 100 range
            migrationBuilder.Sql("""
                                 UPDATE "Maczkopats"
                                 SET "Capacity" = floor(random() * 91 + 10)::int
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Maczkopats");
        }
    }
}
