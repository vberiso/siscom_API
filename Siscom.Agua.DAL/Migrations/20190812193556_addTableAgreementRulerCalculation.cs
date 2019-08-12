using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableAgreementRulerCalculation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 8, 12, 14, 35, 54, 741, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 8, 8, 18, 6, 16, 980, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "agreement_ruler_calculation",
                columns: table => new
                {
                    id_agreement_ruler = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    serviceId = table.Column<int>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    date_in = table.Column<DateTime>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agreement_ruler_calculation", x => x.id_agreement_ruler);
                    table.ForeignKey(
                        name: "FK_agreement_ruler_calculation_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agreement_ruler_calculation_AgreementId",
                table: "agreement_ruler_calculation",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agreement_ruler_calculation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 8, 8, 18, 6, 16, 980, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 8, 12, 14, 35, 54, 741, DateTimeKind.Local));
        }
    }
}
