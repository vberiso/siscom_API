using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addfieldservicios_adelantadosInDebtCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 2, 3, 11, 5, 58, 818, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 2, 2, 22, 50, 39, 72, DateTimeKind.Local));

            migrationBuilder.AddColumn<decimal>(
                name: "servicios_adelantados",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 2, 3, 11, 5, 58, 888, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "servicios_adelantados",
                table: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 2, 2, 22, 50, 39, 72, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 2, 3, 11, 5, 58, 818, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 2, 2, 22, 50, 39, 123, DateTimeKind.Local));
        }
    }
}
