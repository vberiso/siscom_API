using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddFieldDiscountAuthorization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 26, 13, 14, 12, 443, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 25, 17, 55, 9, 526, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "account",
                table: "Discount_Authorization",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Key_Firebase",
                table: "Discount_Authorization",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account",
                table: "Discount_Authorization");

            migrationBuilder.DropColumn(
                name: "Key_Firebase",
                table: "Discount_Authorization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 25, 17, 55, 9, 526, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 26, 13, 14, 12, 443, DateTimeKind.Local));
        }
    }
}
