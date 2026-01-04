using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    public partial class Wiki : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wiki_articles",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<byte[]>(type: "binary(16)", nullable: true),
                    Identifier = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    Title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    DescriptionMember = table.Column<string>(type: "longtext", nullable: true),
                    DescriptionManagementBoard = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wiki_articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wiki_articles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 22, 218, 114, 135, 188, 251, 175, 70, 165, 44, 200, 60, 144, 202, 116, 201 }, "49DD2CBF-AAC9-4015-9016-9E3ED25547DF", "chairperson", "CHAIRPERSON" });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 66, 216, 128, 239, 15, 74, 19, 70, 130, 235, 72, 166, 112, 186, 194, 29 }, "EA00C945-985C-42AD-B149-7D6FBCE9C279", "usermanager", "USERMANAGER" });

            migrationBuilder.CreateIndex(
                name: "IX_wiki_articles_Identifier",
                table: "wiki_articles",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wiki_articles_UserId",
                table: "wiki_articles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wiki_articles");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 22, 218, 114, 135, 188, 251, 175, 70, 165, 44, 200, 60, 144, 202, 116, 201 });

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 66, 216, 128, 239, 15, 74, 19, 70, 130, 235, 72, 166, 112, 186, 194, 29 });
        }
    }
}
