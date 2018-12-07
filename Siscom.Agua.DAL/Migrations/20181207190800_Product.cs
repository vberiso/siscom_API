using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "concept",
                table: "Tariff_Product");

            migrationBuilder.DropColumn(
                name: "percentage",
                table: "Tariff_Product");

            migrationBuilder.DropColumn(
                name: "have_tax",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "in_agreement",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "id_service",
                table: "Product",
                newName: "id_product");

            migrationBuilder.RenameColumn(
                name: "is_service",
                table: "Product",
                newName: "have_tariff");

            migrationBuilder.AddColumn<int>(
                name: "parent",
                table: "Product",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 7, 13, 8, 0, 183, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 6, 15, 55, 53, 809, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parent",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "id_product",
                table: "Product",
                newName: "id_service");

            migrationBuilder.RenameColumn(
                name: "have_tariff",
                table: "Product",
                newName: "is_service");

            migrationBuilder.AddColumn<string>(
                name: "concept",
                table: "Tariff_Product",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "percentage",
                table: "Tariff_Product",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "have_tax",
                table: "Product",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "in_agreement",
                table: "Product",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 6, 15, 55, 53, 809, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 7, 13, 8, 0, 183, DateTimeKind.Local));
        }
    }
}
