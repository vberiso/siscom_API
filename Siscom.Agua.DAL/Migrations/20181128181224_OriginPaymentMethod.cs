using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class OriginPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "debt",
                table: "Payment",
                newName: "id_debt");

            migrationBuilder.RenameColumn(
                name: "authorization",
                table: "Payment",
                newName: "authorization_origin_payment");

            migrationBuilder.AddColumn<string>(
                name: "origin_payment_method",
                table: "Transaction",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "origin_payment_method",
                table: "Payment",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transaction_folio",
                table: "Payment",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 12, 12, 24, 533, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 9, 39, 56, 902, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "is_bank",
                table: "External_Origin_Payment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Debt_Detail",
                columns: table => new
                {
                    id_debt_detail = table.Column<int>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    ServiceId = table.Column<int>(nullable: true),
                    DebtId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Detail", x => x.id_debt_detail);
                    table.ForeignKey(
                        name: "FK_Debt_Detail_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debt_Detail_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_DebtId",
                table: "Debt_Detail",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_ServiceId",
                table: "Debt_Detail",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "origin_payment_method",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "origin_payment_method",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "transaction_folio",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "is_bank",
                table: "External_Origin_Payment");

            migrationBuilder.RenameColumn(
                name: "id_debt",
                table: "Payment",
                newName: "debt");

            migrationBuilder.RenameColumn(
                name: "authorization_origin_payment",
                table: "Payment",
                newName: "authorization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 9, 39, 56, 902, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 12, 12, 24, 533, DateTimeKind.Local));
        }
    }
}
