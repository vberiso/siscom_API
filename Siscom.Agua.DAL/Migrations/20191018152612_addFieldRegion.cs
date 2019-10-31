using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Region",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 18, 10, 26, 12, 279, DateTimeKind.Local).AddTicks(4496),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 9, 16, 57, 31, 434, DateTimeKind.Local).AddTicks(9282));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Region");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 9, 16, 57, 31, 434, DateTimeKind.Local).AddTicks(9282),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 18, 10, 26, 12, 279, DateTimeKind.Local).AddTicks(4496));
        }
    }
}
