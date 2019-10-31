using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTablesPartialPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 28, 18, 7, 30, 907, DateTimeKind.Local).AddTicks(557),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 18, 10, 26, 12, 279, DateTimeKind.Local).AddTicks(4496));

            migrationBuilder.CreateTable(
                name: "partial_payment",
                columns: table => new
                {
                    id_partial_payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 30, nullable: true),
                    partial_payment_date = table.Column<DateTime>(nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    number_of_payments = table.Column<int>(nullable: false),
                    initial_payment = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: true),
                    type_intake = table.Column<string>(maxLength: 50, nullable: true),
                    type_service = table.Column<string>(maxLength: 50, nullable: true),
                    expiration_date = table.Column<DateTime>(type: "date", nullable: false),
                    from_date = table.Column<DateTime>(type: "date", nullable: false),
                    until_date = table.Column<DateTime>(type: "date", nullable: false),
                    observations = table.Column<string>(maxLength: 1000, nullable: true),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partial_payment", x => x.id_partial_payment);
                    table.ForeignKey(
                        name: "FK_partial_payment_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partial_Payment_Debt",
                columns: table => new
                {
                    id_partial_payment_debt = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DebtId = table.Column<int>(nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    on_account = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: true),
                    PartialPaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partial_Payment_Debt", x => x.id_partial_payment_debt);
                    table.ForeignKey(
                        name: "FK_Partial_Payment_Debt_partial_payment_PartialPaymentId",
                        column: x => x.PartialPaymentId,
                        principalTable: "partial_payment",
                        principalColumn: "id_partial_payment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "partial_payment_detail",
                columns: table => new
                {
                    id_partial_payment_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    payment_number = table.Column<int>(nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    on_account = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: true),
                    relase_date = table.Column<DateTime>(nullable: false),
                    relase_debtId = table.Column<int>(nullable: false),
                    payment_date = table.Column<DateTime>(nullable: false),
                    PaymentId = table.Column<int>(nullable: false),
                    PartialPaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partial_payment_detail", x => x.id_partial_payment_detail);
                    table.ForeignKey(
                        name: "FK_partial_payment_detail_partial_payment_PartialPaymentId",
                        column: x => x.PartialPaymentId,
                        principalTable: "partial_payment",
                        principalColumn: "id_partial_payment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partial_Payment_Detail_Concept",
                columns: table => new
                {
                    id_partial_payment_detail_concept = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    on_account = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    code_concept = table.Column<string>(maxLength: 5, nullable: true),
                    name_concept = table.Column<string>(maxLength: 500, nullable: true),
                    PartialPaymentDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partial_Payment_Detail_Concept", x => x.id_partial_payment_detail_concept);
                    table.ForeignKey(
                        name: "FK_Partial_Payment_Detail_Concept_partial_payment_detail_PartialPaymentDetailId",
                        column: x => x.PartialPaymentDetailId,
                        principalTable: "partial_payment_detail",
                        principalColumn: "id_partial_payment_detail",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "partial_payment_detail_status",
                columns: table => new
                {
                    id_partial_payment_detail_status = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    status = table.Column<string>(maxLength: 5, nullable: true),
                    partial_payment_detail_status_date = table.Column<DateTime>(nullable: false),
                    user = table.Column<string>(maxLength: 80, nullable: true),
                    PartialPaymentDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partial_payment_detail_status", x => x.id_partial_payment_detail_status);
                    table.ForeignKey(
                        name: "FK_partial_payment_detail_status_partial_payment_detail_PartialPaymentDetailId",
                        column: x => x.PartialPaymentDetailId,
                        principalTable: "partial_payment_detail",
                        principalColumn: "id_partial_payment_detail",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_partial_payment_AgreementId",
                table: "partial_payment",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Partial_Payment_Debt_PartialPaymentId",
                table: "Partial_Payment_Debt",
                column: "PartialPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_partial_payment_detail_PartialPaymentId",
                table: "partial_payment_detail",
                column: "PartialPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Partial_Payment_Detail_Concept_PartialPaymentDetailId",
                table: "Partial_Payment_Detail_Concept",
                column: "PartialPaymentDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_partial_payment_detail_status_PartialPaymentDetailId",
                table: "partial_payment_detail_status",
                column: "PartialPaymentDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Partial_Payment_Debt");

            migrationBuilder.DropTable(
                name: "Partial_Payment_Detail_Concept");

            migrationBuilder.DropTable(
                name: "partial_payment_detail_status");

            migrationBuilder.DropTable(
                name: "partial_payment_detail");

            migrationBuilder.DropTable(
                name: "partial_payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 18, 10, 26, 12, 279, DateTimeKind.Local).AddTicks(4496),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 28, 18, 7, 30, 907, DateTimeKind.Local).AddTicks(557));
        }
    }
}
