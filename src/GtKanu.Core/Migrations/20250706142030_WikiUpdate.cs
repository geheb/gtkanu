using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    /// <inheritdoc />
    public partial class WikiUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionManagementBoard",
                table: "wiki_articles");

            migrationBuilder.DropColumn(
                name: "DescriptionMember",
                table: "wiki_articles");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "wiki_articles",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "wiki_articles",
                newName: "Created");

            migrationBuilder.AlterColumn<string>(
                name: "Identifier",
                table: "wiki_articles",
                type: "varchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "wiki_articles",
                type: "longtext",
                nullable: false);

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new byte[] { 227, 223, 194, 2, 49, 115, 246, 69, 146, 155, 249, 131, 39, 63, 45, 116 }, "4B04648D-DE82-4B0B-B014-3CE0BE5454FD", "wikimanager", "WIKIMANAGER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 227, 223, 194, 2, 49, 115, 246, 69, 146, 155, 249, 131, 39, 63, 45, 116 });

            migrationBuilder.DropColumn(
                name: "Content",
                table: "wiki_articles");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "wiki_articles",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "wiki_articles",
                newName: "CreatedOn");

            migrationBuilder.AlterColumn<string>(
                name: "Identifier",
                table: "wiki_articles",
                type: "varchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionManagementBoard",
                table: "wiki_articles",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionMember",
                table: "wiki_articles",
                type: "longtext",
                nullable: true);
        }
    }
}
