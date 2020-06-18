using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class editTableORDERWORKLISTAddLatudeLongitude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "latitude",
                table: "order_work_list",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "longitude",
                table: "order_work_list",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 17, 17, 12, 40, 246, DateTimeKind.Local).AddTicks(2363),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 17, 12, 58, 41, 89, DateTimeKind.Local).AddTicks(9096));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "order_work_list");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 17, 12, 58, 41, 89, DateTimeKind.Local).AddTicks(9096),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 17, 17, 12, 40, 246, DateTimeKind.Local).AddTicks(2363));
        }
    }
}
