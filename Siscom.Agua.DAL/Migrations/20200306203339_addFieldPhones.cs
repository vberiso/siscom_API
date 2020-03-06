using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldPhones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "brand",
                table: "phone",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "model",
                table: "phone",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 6, 14, 33, 39, 134, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 5, 16, 6, 33, 884, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "brand",
                table: "phone");

            migrationBuilder.DropColumn(
                name: "model",
                table: "phone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 5, 16, 6, 33, 884, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 6, 14, 33, 39, 134, DateTimeKind.Local));
        }
    }
}
