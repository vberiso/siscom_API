using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TransitPoliceChangeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "TransitPolices",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "TransitPolices",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 15, 9, 31, 5, 488, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 14, 13, 9, 20, 576, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "TransitPolices");

            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "TransitPolices");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 14, 13, 9, 20, 576, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 15, 9, 31, 5, 488, DateTimeKind.Local));
        }
    }
}
