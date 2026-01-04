using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    public partial class Fleet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false),
                    IsInUse = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_bookings",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    VehicleId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    Start = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    BookedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    CancelledOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    Purpose = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicle_bookings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vehicle_bookings_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 96, 48, 73, 249, 184, 48, 243, 75, 164, 190, 116, 99, 11, 15, 164, 35 }, "D8FEF733-1C63-433F-916F-99B8645D1487", "fleetmanager", "FLEETMANAGER" });

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_bookings_UserId",
                table: "vehicle_bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_bookings_VehicleId",
                table: "vehicle_bookings",
                column: "VehicleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicle_bookings");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 96, 48, 73, 249, 184, 48, 243, 75, 164, 190, 116, 99, 11, 15, 164, 35 });
        }
    }
}
