using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldsLatitudeLongitudeObservationToORDERWORKLIST : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "latitude_final",
                table: "order_work_list",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "longitude_final",
                table: "order_work_list",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "observation_final",
                table: "order_work_list",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 23, 12, 31, 55, 834, DateTimeKind.Local).AddTicks(4980),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 23, 12, 1, 53, 66, DateTimeKind.Local).AddTicks(3308));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude_final",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "longitude_final",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "observation_final",
                table: "order_work_list");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 23, 12, 1, 53, 66, DateTimeKind.Local).AddTicks(3308),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 23, 12, 31, 55, 834, DateTimeKind.Local).AddTicks(4980));
        }
    }
}
