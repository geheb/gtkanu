using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    public partial class Boats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boats",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    Location = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    MaxRentalDays = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "boat_rentals",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    BoatId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    Start = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Purpose = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    CancelledOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boat_rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_boat_rentals_boats_BoatId",
                        column: x => x.BoatId,
                        principalTable: "boats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_boat_rentals_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 10, 248, 142, 102, 87, 159, 97, 73, 190, 172, 84, 10, 216, 0, 50, 65 }, "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "boatmanager", "BOATMANAGER" });

            migrationBuilder.CreateIndex(
                name: "IX_boat_rentals_BoatId",
                table: "boat_rentals",
                column: "BoatId");

            migrationBuilder.CreateIndex(
                name: "IX_boat_rentals_UserId",
                table: "boat_rentals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boat_rentals");

            migrationBuilder.DropTable(
                name: "boats");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 10, 248, 142, 102, 87, 159, 97, 73, 190, 172, 84, 10, 216, 0, 50, 65 });
        }
    }
}
