using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFields14ToORDERWORKLIST : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "account",
                table: "order_work_list",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "adeudo",
                table: "order_work_list",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "descuento_vulnerable",
                table: "order_work_list",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "domicilio",
                table: "order_work_list",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "order_work_list",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "have_convenio",
                table: "order_work_list",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nombre",
                table: "order_work_list",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "periodos",
                table: "order_work_list",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ruta",
                table: "order_work_list",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_account",
                table: "order_work_list",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tel",
                table: "order_work_list",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_servicio",
                table: "order_work_list",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_toma",
                table: "order_work_list",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "year",
                table: "order_work_list",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 18, 11, 22, 2, 859, DateTimeKind.Local).AddTicks(8379),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 17, 17, 12, 40, 246, DateTimeKind.Local).AddTicks(2363));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "adeudo",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "descuento_vulnerable",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "domicilio",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "email",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "have_convenio",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "nombre",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "periodos",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "ruta",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "status_account",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "tel",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "tipo_servicio",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "tipo_toma",
                table: "order_work_list");

            migrationBuilder.DropColumn(
                name: "year",
                table: "order_work_list");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 17, 17, 12, 40, 246, DateTimeKind.Local).AddTicks(2363),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 18, 11, 22, 2, 859, DateTimeKind.Local).AddTicks(8379));
        }
    }
}
