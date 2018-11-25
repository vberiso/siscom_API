using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "Derivative",
                columns: table => new
                {
                    id_derivative = table.Column<int>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    AgreementId = table.Column<int>(nullable: true),
                    AgreementDerivative = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derivative", x => x.id_derivative);
                    table.ForeignKey(
                        name: "FK_Derivative_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meter",
                columns: table => new
                {
                    id_meter = table.Column<int>(nullable: false),
                    brand = table.Column<string>(maxLength: 50, nullable: false),
                    model = table.Column<string>(maxLength: 50, nullable: false),
                    consumption = table.Column<string>(maxLength: 10, nullable: true),
                    install_date = table.Column<DateTime>(type: "date", nullable: false),
                    deinstall_date = table.Column<DateTime>(type: "date", nullable: false),
                    serial = table.Column<string>(maxLength: 20, nullable: false),
                    wheels = table.Column<string>(maxLength: 1, nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    AgreementId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meter", x => x.id_meter);
                    table.ForeignKey(
                        name: "FK_Meter_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Origin_Payment",
                columns: table => new
                {
                    Origin_Payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Origin_Payment", x => x.Origin_Payment);
                });

            migrationBuilder.CreateTable(
                name: "Consumption",
                columns: table => new
                {
                    id_consumption = table.Column<int>(nullable: false),
                    consumption_date = table.Column<DateTime>(nullable: false),
                    previous_consumption = table.Column<double>(nullable: false),
                    current_consumption = table.Column<double>(nullable: false),
                    consumption = table.Column<double>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    DebtId = table.Column<int>(nullable: true),
                    MeterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumption", x => x.id_consumption);
                    table.ForeignKey(
                        name: "FK_Consumption_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consumption_Meter_MeterId",
                        column: x => x.MeterId,
                        principalTable: "Meter",
                        principalColumn: "id_meter",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consumption_DebtId",
                table: "Consumption",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumption_MeterId",
                table: "Consumption",
                column: "MeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Derivative_AgreementId",
                table: "Derivative",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Meter_AgreementId",
                table: "Meter",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consumption");

            migrationBuilder.DropTable(
                name: "Derivative");

            migrationBuilder.DropTable(
                name: "Origin_Payment");

            migrationBuilder.DropTable(
                name: "Meter");

           
        }
    }
}
