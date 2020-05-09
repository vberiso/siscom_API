using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterTAbleBreachAddFieldsCodeConceptArrastreAndDaysCorralon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 30, 13, 53, 36, 443, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 29, 12, 26, 12, 67, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "code_concept_arrastre",
                table: "Breach",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "days_corralon",
                table: "Breach",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code_concept_arrastre",
                table: "Breach");

            migrationBuilder.DropColumn(
                name: "days_corralon",
                table: "Breach");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 29, 12, 26, 12, 67, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 30, 13, 53, 36, 443, DateTimeKind.Local));
        }
    }
}
