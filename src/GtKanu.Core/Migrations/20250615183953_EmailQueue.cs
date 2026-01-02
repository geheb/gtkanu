using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmailQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_notifications");

            migrationBuilder.CreateTable(
                name: "email_queue",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Sent = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    Recipient = table.Column<string>(type: "longtext", nullable: false),
                    Subject = table.Column<string>(type: "longtext", nullable: false),
                    HtmlBody = table.Column<string>(type: "longtext", nullable: false),
                    IsPrio = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_queue", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_email_queue_Sent_Created_IsPrio",
                table: "email_queue",
                columns: new[] { "Sent", "Created", "IsPrio" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_queue");

            migrationBuilder.CreateTable(
                name: "account_notifications",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    CallbackUrl = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ReferenceId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    SentOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_notifications_ReferenceId_Type",
                table: "account_notifications",
                columns: new[] { "ReferenceId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_account_notifications_UserId_Type_CreatedOn",
                table: "account_notifications",
                columns: new[] { "UserId", "Type", "CreatedOn" });
        }
    }
}
