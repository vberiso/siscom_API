using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class alterTableAccountAccumulated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 1, 11, 16, 32, 297, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 27, 14, 45, 10, 233, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "isMedido",
                table: "AccountsAccumulated",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "AccountsAccumulated",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isMedido",
                table: "AccountsAccumulated");

            migrationBuilder.DropColumn(
                name: "type",
                table: "AccountsAccumulated");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 27, 14, 45, 10, 233, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 1, 11, 16, 32, 297, DateTimeKind.Local));
        }
    }
}
