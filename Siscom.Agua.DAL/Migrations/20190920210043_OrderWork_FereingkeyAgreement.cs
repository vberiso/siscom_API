using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class OrderWork_FereingkeyAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TechnicalStaffId",
                table: "Order_work",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 20, 16, 0, 42, 877, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 13, 16, 55, 8, 265, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_Order_work_AgreementId",
                table: "Order_work",
                column: "AgreementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_work_TechnicalStaffId",
                table: "Order_work",
                column: "TechnicalStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_work_Agreement_AgreementId",
                table: "Order_work",
                column: "AgreementId",
                principalTable: "Agreement",
                principalColumn: "id_agreement",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_work_Technical_Staff_TechnicalStaffId",
                table: "Order_work",
                column: "TechnicalStaffId",
                principalTable: "Technical_Staff",
                principalColumn: "id_technical_staff",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_work_Agreement_AgreementId",
                table: "Order_work");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_work_Technical_Staff_TechnicalStaffId",
                table: "Order_work");

            migrationBuilder.DropIndex(
                name: "IX_Order_work_AgreementId",
                table: "Order_work");

            migrationBuilder.DropIndex(
                name: "IX_Order_work_TechnicalStaffId",
                table: "Order_work");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicalStaffId",
                table: "Order_work",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 13, 16, 55, 8, 265, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 20, 16, 0, 42, 877, DateTimeKind.Local));
        }
    }
}
