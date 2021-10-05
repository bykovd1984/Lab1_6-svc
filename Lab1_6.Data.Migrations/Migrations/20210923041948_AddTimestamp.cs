using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lab1_6.Data.Migrations.Migrations
{
    public partial class AddTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Orders",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Accounts",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Accounts");
        }
    }
}
