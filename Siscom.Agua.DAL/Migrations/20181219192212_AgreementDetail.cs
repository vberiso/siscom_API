using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 19, 13, 22, 12, 277, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 18, 14, 45, 37, 435, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Agreement_Detail",
                columns: table => new
                {
                    id_agreement_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 50, nullable: false),
                    register = table.Column<string>(maxLength: 50, nullable: false),
                    taxable_base = table.Column<decimal>(nullable: false),
                    ground = table.Column<decimal>(nullable: false),
                    built = table.Column<decimal>(nullable: false),
                    agreement_detail_date = table.Column<DateTime>(nullable: false),
                    last_update = table.Column<DateTime>(nullable: false),
                    sector = table.Column<short>(nullable: false),
                    observation = table.Column<string>(nullable: true),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_Detail", x => x.id_agreement_detail);
                    table.ForeignKey(
                        name: "FK_Agreement_Detail_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Detail_AgreementId",
                table: "Agreement_Detail",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agreement_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 18, 14, 45, 37, 435, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 19, 13, 22, 12, 277, DateTimeKind.Local));
        }
    }
}
