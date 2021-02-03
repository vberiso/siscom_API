using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddFieldsDebtCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 1, 29, 14, 28, 31, 324, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 11, 12, 13, 2, 51, 211, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "DebtCampaign",
                maxLength: 10,
                nullable: true,
                defaultValue: "ECD01",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "ECD01");

            migrationBuilder.AlterColumn<string>(
                name: "servicios",
                table: "DebtCampaign",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "folio",
                table: "DebtCampaign",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DebtId",
                table: "DebtCampaign",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "consumo",
                table: "DebtCampaign",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "account",
                table: "DebtCampaign",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "descuento_multa",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "descuento_notificaciones",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "descuento_recargo",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "importe_multas",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "importe_notificaciones",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "importe_recargo",
                table: "DebtCampaign",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 1, 29, 14, 28, 31, 454, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "descuento_multa",
                table: "DebtCampaign");

            migrationBuilder.DropColumn(
                name: "descuento_notificaciones",
                table: "DebtCampaign");

            migrationBuilder.DropColumn(
                name: "descuento_recargo",
                table: "DebtCampaign");

            migrationBuilder.DropColumn(
                name: "importe_multas",
                table: "DebtCampaign");

            migrationBuilder.DropColumn(
                name: "importe_notificaciones",
                table: "DebtCampaign");

            migrationBuilder.DropColumn(
                name: "importe_recargo",
                table: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 11, 12, 13, 2, 51, 211, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 1, 29, 14, 28, 31, 324, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "DebtCampaign",
                nullable: true,
                defaultValue: "ECD01",
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValue: "ECD01");

            migrationBuilder.AlterColumn<string>(
                name: "servicios",
                table: "DebtCampaign",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "folio",
                table: "DebtCampaign",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DebtId",
                table: "DebtCampaign",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "consumo",
                table: "DebtCampaign",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "account",
                table: "DebtCampaign",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2020, 11, 12, 13, 2, 51, 264, DateTimeKind.Local));
        }
    }
}
