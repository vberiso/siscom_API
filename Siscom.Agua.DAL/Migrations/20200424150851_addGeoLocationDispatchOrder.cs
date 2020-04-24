using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addGeoLocationDispatchOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 24, 10, 8, 51, 59, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 20, 18, 54, 53, 739, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "latitude",
                table: "DispatchOrder",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "longitude",
                table: "DispatchOrder",
                maxLength: 25,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "DispatchOrder");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 20, 18, 54, 53, 739, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 24, 10, 8, 51, 59, DateTimeKind.Local));
        }
    }
}
