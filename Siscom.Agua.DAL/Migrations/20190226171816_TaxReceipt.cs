using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TaxReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "account_number",
                table: "Payment_Detail",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "id_order_sale",
                table: "Payment_Detail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "unit_measurement",
                table: "Payment_Detail",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "have_tax_receipt",
                table: "Payment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 26, 11, 18, 15, 866, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 22, 11, 14, 23, 361, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Tax_Receipt",
                columns: table => new
                {
                    id_tax_receipt = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    tax_receipt_date = table.Column<DateTime>(nullable: false),
                    tax_receipt_xml = table.Column<int>(nullable: false),
                    tax_receipt_xml_fiel = table.Column<int>(nullable: false),
                    rfc = table.Column<string>(maxLength: 17, nullable: true),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_Receipt", x => x.id_tax_receipt);
                    table.ForeignKey(
                        name: "FK_Tax_Receipt_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "id_payment",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tax_Receipt_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tax_Receipt_PaymentId",
                table: "Tax_Receipt",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_Receipt_UserId",
                table: "Tax_Receipt",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tax_Receipt");

            migrationBuilder.DropColumn(
                name: "account_number",
                table: "Payment_Detail");

            migrationBuilder.DropColumn(
                name: "id_order_sale",
                table: "Payment_Detail");

            migrationBuilder.DropColumn(
                name: "unit_measurement",
                table: "Payment_Detail");

            migrationBuilder.DropColumn(
                name: "have_tax_receipt",
                table: "Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 22, 11, 14, 23, 361, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 26, 11, 18, 15, 866, DateTimeKind.Local));
        }
    }
}
