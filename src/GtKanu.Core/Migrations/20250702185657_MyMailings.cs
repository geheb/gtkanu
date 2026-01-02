using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    /// <inheritdoc />
    public partial class MyMailings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "my_mailings",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    MailingId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    HasRead = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_my_mailings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_my_mailings_mailings_MailingId",
                        column: x => x.MailingId,
                        principalTable: "mailings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_my_mailings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_my_mailings_MailingId",
                table: "my_mailings",
                column: "MailingId");

            migrationBuilder.CreateIndex(
                name: "IX_my_mailings_UserId",
                table: "my_mailings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "my_mailings");
        }
    }
}
