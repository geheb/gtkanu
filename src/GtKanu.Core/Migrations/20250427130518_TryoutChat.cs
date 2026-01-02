using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    /// <inheritdoc />
    public partial class TryoutChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tryout_chats",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    TryoutId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Message = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tryout_chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tryout_chats_tryouts_TryoutId",
                        column: x => x.TryoutId,
                        principalTable: "tryouts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tryout_chats_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tryout_chats_TryoutId",
                table: "tryout_chats",
                column: "TryoutId");

            migrationBuilder.CreateIndex(
                name: "IX_tryout_chats_UserId",
                table: "tryout_chats",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tryout_chats");
        }
    }
}
