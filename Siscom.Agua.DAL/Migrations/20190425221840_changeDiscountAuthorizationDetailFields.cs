using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class changeDiscountAuthorizationDetailFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code_concept",
                table: "Discount_Authorization_Detail");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                table: "Discount_Authorization_Detail");

            migrationBuilder.DropColumn(
                name: "discount_percentage",
                table: "Discount_Authorization_Detail");

            migrationBuilder.DropColumn(
                name: "name_concept",
                table: "Discount_Authorization_Detail");

            migrationBuilder.DropColumn(
                name: "original_amount",
                table: "Discount_Authorization_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 25, 17, 18, 39, 184, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 24, 17, 2, 15, 197, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "id_breach",
                table: "Discount_Authorization_Detail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_breach",
                table: "Discount_Authorization_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 24, 17, 2, 15, 197, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 25, 17, 18, 39, 184, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "code_concept",
                table: "Discount_Authorization_Detail",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                table: "Discount_Authorization_Detail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "discount_percentage",
                table: "Discount_Authorization_Detail",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "name_concept",
                table: "Discount_Authorization_Detail",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "original_amount",
                table: "Discount_Authorization_Detail",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
