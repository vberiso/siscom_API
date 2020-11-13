using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class alterLengthFromFieldAliasInCONDONATIONCAMPAAING : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 11, 12, 13, 2, 51, 211, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 10, 15, 52, 3, 969, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "alias",
                table: "CondonationCampaings",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Breach_List",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2020, 11, 12, 13, 2, 51, 264, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 10, 15, 52, 3, 969, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 11, 12, 13, 2, 51, 211, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "alias",
                table: "CondonationCampaings",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Breach_List",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 400);

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2020, 7, 10, 15, 52, 4, 46, DateTimeKind.Local));
        }
    }
}
