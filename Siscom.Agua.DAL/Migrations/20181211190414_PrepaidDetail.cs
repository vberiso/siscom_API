using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class PrepaidDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 11, 13, 4, 14, 18, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 10, 13, 11, 9, 427, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Debt_Discount",
                columns: table => new
                {
                    id_debt_discount = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 5, nullable: false),
                    name_concept = table.Column<string>(maxLength: 150, nullable: false),
                    original_amount = table.Column<double>(nullable: false),
                    discount_amount = table.Column<double>(nullable: false),
                    DebtId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Discount", x => x.id_debt_discount);
                    table.ForeignKey(
                        name: "FK_Debt_Discount_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debt_Prepaid",
                columns: table => new
                {
                    id_debt_pepaid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 5, nullable: false),
                    name_concept = table.Column<string>(maxLength: 150, nullable: false),
                    original_amount = table.Column<double>(nullable: false),
                    payment_amount = table.Column<double>(nullable: false),
                    id_debt = table.Column<int>(nullable: false),
                    PrepaidDetailId = table.Column<int>(nullable: false),
                    PrepaidId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Prepaid", x => x.id_debt_pepaid);
                    table.ForeignKey(
                        name: "FK_Debt_Prepaid_Prepaid_Detail_PrepaidDetailId",
                        column: x => x.PrepaidDetailId,
                        principalTable: "Prepaid_Detail",
                        principalColumn: "id_drepaid_detail",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Prepaid_Prepaid_PrepaidId",
                        column: x => x.PrepaidId,
                        principalTable: "Prepaid",
                        principalColumn: "id_prepaid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Discount_DebtId",
                table: "Debt_Discount",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Prepaid_PrepaidDetailId",
                table: "Debt_Prepaid",
                column: "PrepaidDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Prepaid_PrepaidId",
                table: "Debt_Prepaid",
                column: "PrepaidId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt_Discount");

            migrationBuilder.DropTable(
                name: "Debt_Prepaid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 10, 13, 11, 9, 427, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 11, 13, 4, 14, 18, DateTimeKind.Local));
        }
    }
}
