using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddFieldObservationResponseInAuthorizationDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 30, 16, 5, 32, 844, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 29, 15, 53, 34, 21, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "observation_response",
                table: "Discount_Authorization",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "observation_response",
                table: "Discount_Authorization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 29, 15, 53, 34, 21, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 30, 16, 5, 32, 844, DateTimeKind.Local));
        }
    }
}
