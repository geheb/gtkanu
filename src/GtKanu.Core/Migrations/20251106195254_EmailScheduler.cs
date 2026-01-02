using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmailScheduler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_email_queue_Sent_Created_IsPrio",
                table: "email_queue");

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "email_queue",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextSchedule",
                table: "email_queue",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_queue_NextSchedule_Sent_IsPrio_Created",
                table: "email_queue",
                columns: new[] { "NextSchedule", "Sent", "IsPrio", "Created" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_email_queue_NextSchedule_Sent_IsPrio_Created",
                table: "email_queue");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "email_queue");

            migrationBuilder.DropColumn(
                name: "NextSchedule",
                table: "email_queue");

            migrationBuilder.CreateIndex(
                name: "IX_email_queue_Sent_Created_IsPrio",
                table: "email_queue",
                columns: new[] { "Sent", "Created", "IsPrio" });
        }
    }
}
