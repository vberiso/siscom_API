using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class changeNamesDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Type_Period_ValidityId",
                table: "Discount");

            migrationBuilder.RenameColumn(
                name: "ValidityId",
                table: "Discount",
                newName: "TypePeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_Discount_ValidityId",
                table: "Discount",
                newName: "IX_Discount_TypePeriodId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 17, 49, 39, 766, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 15, 58, 56, 763, DateTimeKind.Local));

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Type_Period_TypePeriodId",
                table: "Discount",
                column: "TypePeriodId",
                principalTable: "Type_Period",
                principalColumn: "id_type_period",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Type_Period_TypePeriodId",
                table: "Discount");

            migrationBuilder.RenameColumn(
                name: "TypePeriodId",
                table: "Discount",
                newName: "ValidityId");

            migrationBuilder.RenameIndex(
                name: "IX_Discount_TypePeriodId",
                table: "Discount",
                newName: "IX_Discount_ValidityId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 15, 58, 56, 763, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 17, 49, 39, 766, DateTimeKind.Local));

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Type_Period_ValidityId",
                table: "Discount",
                column: "ValidityId",
                principalTable: "Type_Period",
                principalColumn: "id_type_period",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
