using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class OrderSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_tax_user",
                table: "Order_Sale",
                newName: "TaxUserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 28, 15, 2, 52, 384, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 28, 10, 56, 15, 172, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_Order_Sale_TaxUserId",
                table: "Order_Sale",
                column: "TaxUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Sale_Tax_User_TaxUserId",
                table: "Order_Sale",
                column: "TaxUserId",
                principalTable: "Tax_User",
                principalColumn: "id_tax_user",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Sale_Tax_User_TaxUserId",
                table: "Order_Sale");

            migrationBuilder.DropIndex(
                name: "IX_Order_Sale_TaxUserId",
                table: "Order_Sale");

            migrationBuilder.RenameColumn(
                name: "TaxUserId",
                table: "Order_Sale",
                newName: "id_tax_user");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 28, 10, 56, 15, 172, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 28, 15, 2, 52, 384, DateTimeKind.Local));
        }
    }
}
