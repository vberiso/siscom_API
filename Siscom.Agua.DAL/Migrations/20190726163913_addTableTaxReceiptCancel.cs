using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableTaxReceiptCancel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 26, 11, 39, 13, 317, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 23, 14, 25, 28, 302, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Tax_Receipt_Cancel",
                columns: table => new
                {
                    id_tax_receipt_cancel = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    status = table.Column<string>(maxLength: 100, nullable: true),
                    message = table.Column<string>(maxLength: 200, nullable: true),
                    requestDateCancel = table.Column<DateTime>(nullable: false),
                    cancelationDate = table.Column<DateTime>(nullable: false),
                    acuseXml = table.Column<byte[]>(nullable: true),
                    TaxReceiptId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_Receipt_Cancel", x => x.id_tax_receipt_cancel);
                    table.ForeignKey(
                        name: "FK_Tax_Receipt_Cancel_Tax_Receipt_TaxReceiptId",
                        column: x => x.TaxReceiptId,
                        principalTable: "Tax_Receipt",
                        principalColumn: "id_tax_receipt",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tax_Receipt_Cancel_TaxReceiptId",
                table: "Tax_Receipt_Cancel",
                column: "TaxReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tax_Receipt_Cancel");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 23, 14, 25, 28, 302, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 26, 11, 39, 13, 317, DateTimeKind.Local));
        }
    }
}
