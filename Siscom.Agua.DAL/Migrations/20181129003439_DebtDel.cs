using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtDel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumption_Debt_DebtId",
                table: "Consumption");

            migrationBuilder.DropForeignKey(
                name: "FK_Debt_Detail_Debt_DebtId",
                table: "Debt_Detail");

            migrationBuilder.DropTable(
                name: "Debt");

            migrationBuilder.DropIndex(
                name: "IX_Debt_Detail_DebtId",
                table: "Debt_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Consumption_DebtId",
                table: "Consumption");

            migrationBuilder.DropColumn(
                name: "DebtId",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "DebtId",
                table: "Consumption");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 18, 34, 38, 837, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 18, 29, 56, 44, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 18, 29, 56, 44, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 18, 34, 38, 837, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "DebtId",
                table: "Debt_Detail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DebtId",
                table: "Consumption",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Debt",
                columns: table => new
                {
                    id_debt = table.Column<int>(nullable: false),
                    AgreementId = table.Column<int>(nullable: true),
                    amount = table.Column<double>(nullable: false),
                    consumption = table.Column<string>(maxLength: 10, nullable: false),
                    debit_date = table.Column<DateTime>(nullable: false),
                    DebtPeriodId = table.Column<int>(nullable: true),
                    derivatives = table.Column<int>(nullable: false),
                    discount = table.Column<string>(maxLength: 50, nullable: true),
                    from_date = table.Column<DateTime>(type: "date", nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    type_intake = table.Column<string>(maxLength: 50, nullable: false),
                    type_service = table.Column<string>(maxLength: 50, nullable: false),
                    until_date = table.Column<DateTime>(type: "date", nullable: false),
                    year = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt", x => x.id_debt);
                    table.ForeignKey(
                        name: "FK_Debt_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debt_Debt_Period_DebtPeriodId",
                        column: x => x.DebtPeriodId,
                        principalTable: "Debt_Period",
                        principalColumn: "id_debt_period",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_DebtId",
                table: "Debt_Detail",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumption_DebtId",
                table: "Consumption",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_AgreementId",
                table: "Debt",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_DebtPeriodId",
                table: "Debt",
                column: "DebtPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumption_Debt_DebtId",
                table: "Consumption",
                column: "DebtId",
                principalTable: "Debt",
                principalColumn: "id_debt",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Debt_Detail_Debt_DebtId",
                table: "Debt_Detail",
                column: "DebtId",
                principalTable: "Debt",
                principalColumn: "id_debt",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
