using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.backend.YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20250808_HomeAndSettings_Sync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4479), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4480) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4503), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4503) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4514), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4514) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4524), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4524) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4535), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4535) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4734), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4734) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4748), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4748) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4758), new DateTime(2025, 8, 7, 23, 55, 10, 161, DateTimeKind.Utc).AddTicks(4758) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9542), new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9543) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9566), new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9566) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9581), new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9581) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9597), new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9598) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9614), new DateTime(2025, 8, 7, 23, 55, 10, 95, DateTimeKind.Utc).AddTicks(9614) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7790), new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7791) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7816), new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7817) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7828), new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7828) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7839), new DateTime(2025, 8, 7, 23, 55, 10, 93, DateTimeKind.Utc).AddTicks(7839) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9343), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9343), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9343) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9348), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9349), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9349) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9191), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9192), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9172) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9230), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9232), new DateTime(2025, 8, 7, 23, 55, 10, 210, DateTimeKind.Utc).AddTicks(9226) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4509), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4509) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4528), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4529) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4541), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4541) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4552), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4552) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4562), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4562) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4573), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4573) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4583), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4584) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4594), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4595) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2153), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2153) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2187), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2187) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2199), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2199) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2222), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2222) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2233), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2233) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3458), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3458) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3483), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3483) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3494), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3494) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3504), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3505) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5740), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5741), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5741) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5745), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5745), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5745) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5587), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5588), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5580) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5630), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5631), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5628) });
        }
    }
}
