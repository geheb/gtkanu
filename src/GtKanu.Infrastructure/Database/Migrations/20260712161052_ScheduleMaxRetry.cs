using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleMaxRetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "email_queue",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "email_queue");
        }
    }
}
