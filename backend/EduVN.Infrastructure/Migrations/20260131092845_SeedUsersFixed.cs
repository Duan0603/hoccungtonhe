using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduVN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "GoogleId", "Grade", "PasswordHash", "Role", "School", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@hoccungtonhe.com", "Admin User", null, null, "$2a$11$eIEAErt55HWnnDvJE1DOP.9MKV70fzW.96e7sFvv6D6Wmi7T3EblK", "Admin", null, "Approved", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "teacher@hoccungtonhe.com", "Instructor User", null, null, "$2a$11$SkF1AimECPvz0goq3S/tR.d1s/TDTDM6pvJlraZC8KHcGQN2YhqZm", "Instructor", null, "Approved", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student@hoccungtonhe.com", "Student User", null, null, "$2a$11$LQhhIwaxOp6hp9wkuYFTi.ZjFCS5Q/JftH9SudOKje7SK7TRAuGxu", "Student", null, "Approved", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}
