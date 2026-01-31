using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduVN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderCode",
                table: "orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "orders");
        }
    }
}
