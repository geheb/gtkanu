using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GtKanu.Core.Migrations
{
    public partial class Seeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new byte[] { 165, 13, 169, 3, 240, 171, 233, 69, 155, 128, 205, 172, 121, 11, 113, 5 }, "956824FE-2F13-4919-B2D2-0E60BECFCA12", "member", "MEMBER" },
                    { new byte[] { 224, 7, 77, 29, 174, 64, 203, 69, 164, 246, 57, 118, 114, 243, 96, 91 }, "995137CB-BD7B-4747-884E-A7467F3C0A3A", "kitchen", "KITCHEN" },
                    { new byte[] { 118, 69, 182, 31, 144, 63, 233, 76, 140, 58, 207, 161, 63, 88, 113, 103 }, "6EE38FDC-5CE5-42FD-A5A5-168573DB2F86", "treasurer", "TREASURER" },
                    { new byte[] { 191, 247, 188, 251, 191, 140, 15, 68, 147, 156, 246, 118, 49, 16, 154, 160 }, "CFAD6F62-EEAA-4ECD-B847-0762C704EC45", "administrator", "ADMINISTRATOR" },
                    { new byte[] { 167, 240, 198, 157, 22, 113, 32, 72, 139, 118, 79, 32, 183, 48, 83, 238 }, "3096E774-529C-41B4-8CD3-355E0D2C930D", "interested", "INTERESTED" },
                    { new byte[] { 236, 155, 220, 45, 150, 235, 213, 76, 148, 180, 221, 13, 153, 161, 150, 100 }, "F3BFC39C-70B7-4E91-A70B-131925290446", "tripmanager", "TRIPMANAGER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 165, 13, 169, 3, 240, 171, 233, 69, 155, 128, 205, 172, 121, 11, 113, 5 });

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 224, 7, 77, 29, 174, 64, 203, 69, 164, 246, 57, 118, 114, 243, 96, 91 });

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 118, 69, 182, 31, 144, 63, 233, 76, 140, 58, 207, 161, 63, 88, 113, 103 });

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 191, 247, 188, 251, 191, 140, 15, 68, 147, 156, 246, 118, 49, 16, 154, 160 });

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 167, 240, 198, 157, 22, 113, 32, 72, 139, 118, 79, 32, 183, 48, 83, 238 });

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new byte[] { 236, 155, 220, 45, 150, 235, 213, 76, 148, 180, 221, 13, 153, 161, 150, 100 });
        }
    }
}
