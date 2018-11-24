using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTypeIntakeFieldAcronym : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "intake_acronym",
                table: "Type_Use",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "intake_acronym",
                table: "Type_Regime",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "acronym",
                table: "Type_Intake",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "intake_acronym",
                table: "Type_Commertial_Business",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 14, 24, 54, 210, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 21, 12, 31, 10, 6, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "Lon",
                table: "Address",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "lat",
                table: "Address",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "intake_acronym",
                table: "Type_Use");

            migrationBuilder.DropColumn(
                name: "intake_acronym",
                table: "Type_Regime");

            migrationBuilder.DropColumn(
                name: "acronym",
                table: "Type_Intake");

            migrationBuilder.DropColumn(
                name: "intake_acronym",
                table: "Type_Commertial_Business");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 12, 31, 10, 6, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 21, 14, 24, 54, 210, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "Lon",
                table: "Address",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "lat",
                table: "Address",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
