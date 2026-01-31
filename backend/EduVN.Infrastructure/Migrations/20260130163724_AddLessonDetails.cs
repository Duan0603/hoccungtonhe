using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduVN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "lessons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "lessons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "lessons");
        }
    }
}
