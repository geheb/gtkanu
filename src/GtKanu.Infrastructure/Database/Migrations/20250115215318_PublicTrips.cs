using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class PublicTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Categories",
                table: "trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "trips",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "trips");
        }
    }
}
