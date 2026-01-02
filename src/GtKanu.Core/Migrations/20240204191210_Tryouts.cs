using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    public partial class Tryouts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tryouts",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    MaxBookings = table.Column<int>(type: "int", nullable: false),
                    BookingStart = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    BookingEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tryouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tryouts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tryout_bookings",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    TryoutId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    BookedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    CancelledOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tryout_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tryout_bookings_tryouts_TryoutId",
                        column: x => x.TryoutId,
                        principalTable: "tryouts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tryout_bookings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tryout_bookings_TryoutId",
                table: "tryout_bookings",
                column: "TryoutId");

            migrationBuilder.CreateIndex(
                name: "IX_tryout_bookings_UserId",
                table: "tryout_bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tryouts_UserId",
                table: "tryouts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tryout_bookings");

            migrationBuilder.DropTable(
                name: "tryouts");
        }
    }
}
