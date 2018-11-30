using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ExternalOriginPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "authorization_origin_payment",
                table: "Transaction",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_debt",
                table: "Transaction",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExternalOriginPaymentId",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginPaymentId",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalOriginPaymentId",
                table: "Payment",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 29, 14, 20, 47, 855, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 29, 13, 36, 53, 524, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ExternalOriginPaymentId",
                table: "Transaction",
                column: "ExternalOriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_OriginPaymentId",
                table: "Transaction",
                column: "OriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ExternalOriginPaymentId",
                table: "Payment",
                column: "ExternalOriginPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_External_Origin_Payment_ExternalOriginPaymentId",
                table: "Payment",
                column: "ExternalOriginPaymentId",
                principalTable: "External_Origin_Payment",
                principalColumn: "id_external_origin_payment",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_External_Origin_Payment_ExternalOriginPaymentId",
                table: "Transaction",
                column: "ExternalOriginPaymentId",
                principalTable: "External_Origin_Payment",
                principalColumn: "id_external_origin_payment",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Origin_Payment_OriginPaymentId",
                table: "Transaction",
                column: "OriginPaymentId",
                principalTable: "Origin_Payment",
                principalColumn: "id_origin_payment",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_External_Origin_Payment_ExternalOriginPaymentId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_External_Origin_Payment_ExternalOriginPaymentId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Origin_Payment_OriginPaymentId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ExternalOriginPaymentId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_OriginPaymentId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Payment_ExternalOriginPaymentId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "authorization_origin_payment",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "id_debt",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ExternalOriginPaymentId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "OriginPaymentId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ExternalOriginPaymentId",
                table: "Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 29, 13, 36, 53, 524, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 29, 14, 20, 47, 855, DateTimeKind.Local));
        }
    }
}
