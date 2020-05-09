using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ALterTableBreachAddDaysCorralon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 4, 11, 38, 54, 32, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 30, 13, 53, 36, 443, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "code_concept_days_corralon",
                table: "Breach",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code_concept_days_corralon",
                table: "Breach");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 13, 53, 36, 443, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 4, 11, 38, 54, 32, DateTimeKind.Local));
        }
    }
}
