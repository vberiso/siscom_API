using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ServiceConcepts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Service",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 25);

            migrationBuilder.AddColumn<bool>(
                name: "have_tax",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "in_agreement",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_service",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "order",
                table: "Service",
                nullable: false,
                defaultValue: (short)0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "have_tax",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "in_agreement",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "is_service",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "order",
                table: "Service");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Service",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
