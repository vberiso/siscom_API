using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTableDebtCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 27, 16, 36, 50, 147, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 10, 13, 8, 35, 268, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "DebtCampaign",
                columns: table => new
                {
                    id_debt_campaign = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ruta = table.Column<int>(nullable: false),
                    AgreementId = table.Column<int>(nullable: false),
                    account = table.Column<string>(nullable: true),
                    start_year_debt = table.Column<int>(nullable: false),
                    end_year_debt = table.Column<int>(nullable: false),
                    importe = table.Column<decimal>(nullable: false),
                    iva = table.Column<decimal>(nullable: false),
                    total = table.Column<decimal>(nullable: false),
                    total_agua = table.Column<decimal>(nullable: false),
                    total_drenaje = table.Column<decimal>(nullable: false),
                    total_saneamiento = table.Column<decimal>(nullable: false),
                    status = table.Column<string>(nullable: true, defaultValue: "ECD01"),
                    folio = table.Column<string>(nullable: true),
                    date_subscription = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtCampaign", x => x.id_debt_campaign);
                    table.ForeignKey(
                        name: "FK_DebtCampaign_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtCampaign_AgreementId",
                table: "DebtCampaign",
                column: "AgreementId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 10, 13, 8, 35, 268, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 27, 16, 36, 50, 147, DateTimeKind.Local));
        }
    }
}
