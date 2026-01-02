using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    public partial class Invoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "InvoiceId",
                table: "bookings",
                type: "binary(16)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaidOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invoices_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_InvoiceId",
                table: "bookings",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_UserId",
                table: "invoices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_invoices_InvoiceId",
                table: "bookings",
                column: "InvoiceId",
                principalTable: "invoices",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_invoices_InvoiceId",
                table: "bookings");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_bookings_InvoiceId",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "bookings");
        }
    }
}
