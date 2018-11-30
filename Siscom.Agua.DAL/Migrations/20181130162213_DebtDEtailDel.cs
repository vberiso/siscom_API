using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtDEtailDel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "id_debt",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "origin_payment_method",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "origin_payment_method",
                table: "Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 30, 10, 22, 13, 218, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 29, 14, 20, 47, 855, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_debt",
                table: "Transaction",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 29, 14, 20, 47, 855, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 30, 10, 22, 13, 218, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Debt_Detail",
                columns: table => new
                {
                    id_debt_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<double>(nullable: false),
                    DebtId = table.Column<int>(nullable: true),
                    have_tax = table.Column<bool>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    ServiceId = table.Column<int>(nullable: true)
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
    }
}
