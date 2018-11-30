using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtDetailService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 30, 10, 29, 55, 28, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 30, 10, 22, 13, 218, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Debt_Detail",
                columns: table => new
                {
                    amount = table.Column<double>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false),
                    DebtId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Detail", x => new { x.DebtId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_Debt_Detail_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Detail_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_ServiceId",
                table: "Debt_Detail",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 30, 10, 22, 13, 218, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 30, 10, 29, 55, 28, DateTimeKind.Local));
        }
    }
}
