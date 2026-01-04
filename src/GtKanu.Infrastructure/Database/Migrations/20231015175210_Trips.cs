using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    public partial class Trips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    Target = table.Column<string>(type: "longtext", nullable: false),
                    MaxBookings = table.Column<int>(type: "int", nullable: false),
                    BookingStart = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    BookingEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trips_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "trip_bookings",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    TripId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    BookedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    CancelledOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trip_bookings_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_trip_bookings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_bookings_TripId",
                table: "trip_bookings",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_trip_bookings_UserId",
                table: "trip_bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_UserId",
                table: "trips",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_Start_End",
                table: "trips",
                columns: new[] { "Start", "End" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_bookings");

            migrationBuilder.DropTable(
                name: "trips");
        }
    }
}
