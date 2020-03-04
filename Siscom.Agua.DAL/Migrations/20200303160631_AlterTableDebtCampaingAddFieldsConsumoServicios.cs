using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterTableDebtCampaingAddFieldsConsumoServicios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 3, 10, 6, 30, 812, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 29, 12, 11, 44, 939, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "consumo",
                table: "DebtCampaign",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "servicios",
                table: "DebtCampaign",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "consumo",
                table: "DebtCampaign");

            migrationBuilder.DropColumn(
                name: "servicios",
                table: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 29, 12, 11, 44, 939, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 3, 10, 6, 30, 812, DateTimeKind.Local));
        }
    }
}
