using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldObservationsIntoGroupType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "observations",
                table: "Group_Type",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 22, 12, 19, 7, 824, DateTimeKind.Local).AddTicks(1609),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 12, 16, 19, 20, 132, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "observations",
                table: "Group_Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 12, 16, 19, 20, 132, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 22, 12, 19, 7, 824, DateTimeKind.Local).AddTicks(1609));
        }
    }
}
