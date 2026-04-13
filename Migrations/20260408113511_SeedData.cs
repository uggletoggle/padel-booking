using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PadelBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "Address", "CloseTime", "CreatedAt", "IsActive", "Name", "OpenTime" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "123 Padel St, Sports City", new TimeOnly(23, 0, 0), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Padel Pro Club", new TimeOnly(8, 0, 0) });

            migrationBuilder.InsertData(
                table: "Courts",
                columns: new[] { "Id", "ClubId", "CreatedAt", "IsActive", "IsCovered", "LayoutPositionX", "LayoutPositionY", "LayoutRotationZ", "Name", "SurfaceType" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 0.0, 0.0, 0.0, "Court 1 (Glass)", "Artificial Grass" });

            migrationBuilder.InsertData(
                table: "Courts",
                columns: new[] { "Id", "ClubId", "CreatedAt", "IsActive", "LayoutPositionX", "LayoutPositionY", "LayoutRotationZ", "Name", "SurfaceType" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 15.0, 0.0, 0.0, "Court 2 (WPT)", "Mondo Supercourt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
