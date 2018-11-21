using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeContactAndClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_movil",
                table: "Contact");

            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 11, 24, 19, 194, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 20, 12, 14, 30, 958, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "type_number",
                table: "Contact",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ine",
                table: "Client",
                maxLength: 13,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 13);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type_number",
                table: "Contact");

            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 20, 12, 14, 30, 958, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 21, 11, 24, 19, 194, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "is_movil",
                table: "Contact",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ine",
                table: "Client",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 13,
                oldNullable: true);
        }
    }
}
