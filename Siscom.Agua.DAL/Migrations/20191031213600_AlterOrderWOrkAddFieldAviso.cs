using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterOrderWOrkAddFieldAviso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "aviso",
                table: "Order_work",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 31, 15, 35, 59, 149, DateTimeKind.Local).AddTicks(6553),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 11, 13, 11, 35, 793, DateTimeKind.Local).AddTicks(4305));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aviso",
                table: "Order_work");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 11, 13, 11, 35, 793, DateTimeKind.Local).AddTicks(4305),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 31, 15, 35, 59, 149, DateTimeKind.Local).AddTicks(6553));
        }
    }
}
