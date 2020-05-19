using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class changeNamesFieldFromUnitMeasurement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IActive",
                table: "UnitMeasurements",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "MaterialLists",
                newName: "is_active");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 18, 16, 55, 21, 25, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 18, 15, 49, 38, 611, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "UnitMeasurements",
                newName: "IActive");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "MaterialLists",
                newName: "IsActive");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 18, 15, 49, 38, 611, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 18, 16, 55, 21, 25, DateTimeKind.Local));
        }
    }
}
