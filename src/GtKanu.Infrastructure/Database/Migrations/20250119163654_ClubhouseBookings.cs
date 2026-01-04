using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ClubhouseBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clubhouse_bookings",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Title = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clubhouse_bookings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 117, 14, 212, 175, 254, 220, 70, 76, 157, 76, 255, 112, 31, 34, 81, 121 }, "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "housemanager", "HOUSEMANAGER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clubhouse_bookings");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 117, 14, 212, 175, 254, 220, 70, 76, 157, 76, 255, 112, 31, 34, 81, 121 });
        }
    }
}
