using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddFieldDebCampaignAndChangeTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 1, 29, 17, 9, 21, 292, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 1, 29, 14, 28, 31, 324, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "start_year_debt",
                table: "DebtCampaign",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "end_year_debt",
                table: "DebtCampaign",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<decimal>(
                name: "total_descuento_servicios",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 1, 29, 17, 9, 21, 342, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_descuento_servicios",
                table: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 1, 29, 14, 28, 31, 324, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 1, 29, 17, 9, 21, 292, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "start_year_debt",
                table: "DebtCampaign",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "end_year_debt",
                table: "DebtCampaign",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 1, 29, 14, 28, 31, 454, DateTimeKind.Local));
        }
    }
}
