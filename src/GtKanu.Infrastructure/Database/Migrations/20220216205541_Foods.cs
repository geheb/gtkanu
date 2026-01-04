using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    public partial class Foods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "food_lists",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_lists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "foods",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FoodListId = table.Column<byte[]>(type: "binary(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_foods_food_lists_FoodListId",
                        column: x => x.FoodListId,
                        principalTable: "food_lists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_food_lists_ValidFrom",
                table: "food_lists",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_foods_FoodListId",
                table: "foods",
                column: "FoodListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "foods");

            migrationBuilder.DropTable(
                name: "food_lists");
        }
    }
}
