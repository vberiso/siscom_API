using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AuthorizationDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 22, 18, 5, 56, 529, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 3, 18, 13, 23, 313, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "DiscountAuthorizationId",
                table: "Discount_Authorization_Detail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Authorization_Detail_DiscountAuthorizationId",
                table: "Discount_Authorization_Detail",
                column: "DiscountAuthorizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Authorization_Detail_Discount_Authorization_DiscountAuthorizationId",
                table: "Discount_Authorization_Detail",
                column: "DiscountAuthorizationId",
                principalTable: "Discount_Authorization",
                principalColumn: "id_discount_authorization",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Authorization_Detail_Discount_Authorization_DiscountAuthorizationId",
                table: "Discount_Authorization_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Discount_Authorization_Detail_DiscountAuthorizationId",
                table: "Discount_Authorization_Detail");

            migrationBuilder.DropColumn(
                name: "DiscountAuthorizationId",
                table: "Discount_Authorization_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 3, 18, 13, 23, 313, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 22, 18, 5, 56, 529, DateTimeKind.Local));
        }
    }
}
