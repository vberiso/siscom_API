using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class PaymentDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_debt",
                table: "Payment",
                newName: "id_agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 13, 10, 44, 43, 888, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 12, 16, 42, 34, 226, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Payment_Detail",
                columns: table => new
                {
                    id_transaction_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 10, nullable: true),
                    description = table.Column<string>(maxLength: 150, nullable: true),
                    amount = table.Column<double>(nullable: false),
                    id_debt = table.Column<int>(nullable: false),
                    id_prepaid = table.Column<int>(nullable: false),
                    PaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Detail", x => x.id_transaction_detail);
                    table.ForeignKey(
                        name: "FK_Payment_Detail_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "id_payment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Detail_PaymentId",
                table: "Payment_Detail",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment_Detail");

            migrationBuilder.RenameColumn(
                name: "id_agreement",
                table: "Payment",
                newName: "id_debt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 12, 16, 42, 34, 226, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 13, 10, 44, 43, 888, DateTimeKind.Local));
        }
    }
}
