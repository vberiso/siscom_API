using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class changeDiscountAuthorizationDetailFieldsBreach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_breach",
                table: "Discount_Authorization_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 25, 17, 55, 9, 526, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 25, 17, 18, 39, 184, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 25, 17, 18, 39, 184, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 25, 17, 55, 9, 526, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "id_breach",
                table: "Discount_Authorization_Detail",
                nullable: false,
                defaultValue: 0);
        }
    }
}
