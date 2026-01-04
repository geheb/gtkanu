using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Mailing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mailings",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    CanSendToAllMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OtherRecipients = table.Column<string>(type: "longtext", nullable: true),
                    ReplyAddress = table.Column<string>(type: "longtext", nullable: true),
                    Subject = table.Column<string>(type: "longtext", nullable: false),
                    HtmlBody = table.Column<string>(type: "longtext", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EmailCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mailings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 43, 68, 201, 109, 121, 50, 204, 70, 149, 192, 29, 79, 140, 61, 49, 240 }, "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "mailingmanager", "MAILINGMANAGER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mailings");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 43, 68, 201, 109, 121, 50, 204, 70, 149, 192, 29, 79, 140, 61, 49, 240 });
        }
    }
}
