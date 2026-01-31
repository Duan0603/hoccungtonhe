using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduVN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$eIEAErt55HWnnDvJE1DOP.9MKV70fzW.96e7sFvv6D6Wmi7T3EblK");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$SkF1AimECPvz0goq3S/tR.d1s/TDTDM6pvJlraZC8KHcGQN2YhqZm");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "$2a$11$LQhhIwaxOp6hp9wkuYFTi.ZjFCS5Q/JftH9SudOKje7SK7TRAuGxu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$9lbwmASrB1KYjOvQ.hSK7.eMWyTWjICvRBNJixIUVq43lYrpu5KwC");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$6o.TfpoNGYfK/Uu/pxPeVeEckg4jlUG1XOx36EGAI2S7/p2.eIjDO");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "$2a$11$bYPAl4mKgdVbPs4lyy.Og.bhltDftyY8UNTLu.3i5fDej7UyU/5b.");
        }
    }
}
