using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddFieldTokenAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "cve_ciudad",
                table: "Postal_Mx",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<int>(
                name: "cod_cp",
                table: "Postal_Mx",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 9, 16, 57, 31, 434, DateTimeKind.Local).AddTicks(9282),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 8, 14, 0, 0, 4, DateTimeKind.Local).AddTicks(8676));

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "Agreement",
                maxLength: 11,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "token",
                table: "Agreement");

            migrationBuilder.AlterColumn<int>(
                name: "cve_ciudad",
                table: "Postal_Mx",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "cod_cp",
                table: "Postal_Mx",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 8, 14, 0, 0, 4, DateTimeKind.Local).AddTicks(8676),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 9, 16, 57, 31, 434, DateTimeKind.Local).AddTicks(9282));
        }
    }
}
