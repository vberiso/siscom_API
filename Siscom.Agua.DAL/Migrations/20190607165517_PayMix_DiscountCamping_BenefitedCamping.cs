using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class PayMix_DiscountCamping_BenefitedCamping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "bank_draft_payment",
                table: "Payment",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "caed_payment",
                table: "Payment",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "cash_payment",
                table: "Payment",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "transference_payment",
                table: "Payment",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 7, 11, 55, 16, 377, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 5, 30, 14, 16, 27, 722, DateTimeKind.Local));

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Discount_Campaign",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Discount_Campaign",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BenefitedCampaign",
                columns: table => new
                {
                    id_discount_campaign = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    agreementId = table.Column<int>(nullable: false),
                    discount_campaignId = table.Column<int>(nullable: false),
                    name_camping = table.Column<string>(maxLength: 75, nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "date", nullable: false),
                    AmountDiscount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitedCampaign", x => x.id_discount_campaign);
                });

            migrationBuilder.CreateTable(
                name: "DetailOfPaymentMethods",
                columns: table => new
                {
                    id_detail_payment_method = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    card_number = table.Column<string>(nullable: true),
                    authorization_bank = table.Column<string>(nullable: true),
                    check_issuance_series = table.Column<string>(nullable: true),
                    account_number = table.Column<string>(nullable: true),
                    tracking_number = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: true),
                    PaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailOfPaymentMethods", x => x.id_detail_payment_method);
                    table.ForeignKey(
                        name: "FK_DetailOfPaymentMethods_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "id_payment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailOfPaymentMethods_PaymentId",
                table: "DetailOfPaymentMethods",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenefitedCampaign");

            migrationBuilder.DropTable(
                name: "DetailOfPaymentMethods");

            migrationBuilder.DropColumn(
                name: "bank_draft_payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "caed_payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "cash_payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "transference_payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Discount_Campaign");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Discount_Campaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 5, 30, 14, 16, 27, 722, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 7, 11, 55, 16, 377, DateTimeKind.Local));
        }
    }
}
