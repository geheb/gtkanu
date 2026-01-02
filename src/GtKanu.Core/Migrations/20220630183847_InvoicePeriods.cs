using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    public partial class InvoicePeriods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "InvoicePeriodId",
                table: "invoices",
                type: "binary(16)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "invoice_periods",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    From = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_periods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoices_InvoicePeriodId",
                table: "invoices",
                column: "InvoicePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_periods_To",
                table: "invoice_periods",
                column: "To");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_invoice_periods_InvoicePeriodId",
                table: "invoices",
                column: "InvoicePeriodId",
                principalTable: "invoice_periods",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_invoice_periods_InvoicePeriodId",
                table: "invoices");

            migrationBuilder.DropTable(
                name: "invoice_periods");

            migrationBuilder.DropIndex(
                name: "IX_invoices_InvoicePeriodId",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "InvoicePeriodId",
                table: "invoices");
        }
    }
}
