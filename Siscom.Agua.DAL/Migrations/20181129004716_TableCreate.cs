using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TableCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 18, 47, 16, 84, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 18, 41, 18, 781, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Debt_Detail",
                columns: table => new
                {
                    id_debt_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<double>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    ServiceId = table.Column<int>(nullable: true),
                    DebtId = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Derivative",
                columns: table => new
                {
                    id_derivative = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    id_meter = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                name: "Consumption",
                columns: table => new
                {
                    id_consumption = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                name: "IX_Debt_Detail_DebtId",
                table: "Debt_Detail",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_ServiceId",
                table: "Debt_Detail",
                column: "ServiceId");

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
                name: "Debt_Detail");

            migrationBuilder.DropTable(
                name: "Derivative");

            migrationBuilder.DropTable(
                name: "Meter");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 18, 41, 18, 781, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 18, 47, 16, 84, DateTimeKind.Local));
        }
    }
}
