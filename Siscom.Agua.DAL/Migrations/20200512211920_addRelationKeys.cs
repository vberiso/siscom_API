using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addRelationKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Type",
                table: "Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 12, 16, 19, 20, 132, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 4, 11, 38, 54, 32, DateTimeKind.Local));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Type",
                table: "Type",
                columns: new[] { "id_type", "GroupTypeId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Type",
                table: "Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 4, 11, 38, 54, 32, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 12, 16, 19, 20, 132, DateTimeKind.Local));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Type",
                table: "Type",
                column: "id_type");
        }
    }
}
