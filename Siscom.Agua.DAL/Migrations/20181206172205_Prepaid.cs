using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Prepaid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_group_type",
                table: "Group_Status",
                newName: "id_group_status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 6, 11, 22, 5, 329, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 5, 17, 10, 5, 14, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Prepaid",
                columns: table => new
                {
                    id_prepaid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prepaid_date = table.Column<DateTime>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    accredited = table.Column<double>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prepaid", x => x.id_prepaid);
                    table.ForeignKey(
                        name: "FK_Prepaid_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prepaid_Detail",
                columns: table => new
                {
                    id_drepaid_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prepaid_detail_date = table.Column<DateTime>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    PrepaidId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prepaid_Detail", x => x.id_drepaid_detail);
                    table.ForeignKey(
                        name: "FK_Prepaid_Detail_Prepaid_PrepaidId",
                        column: x => x.PrepaidId,
                        principalTable: "Prepaid",
                        principalColumn: "id_prepaid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prepaid_AgreementId",
                table: "Prepaid",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Prepaid_Detail_PrepaidId",
                table: "Prepaid_Detail",
                column: "PrepaidId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prepaid_Detail");

            migrationBuilder.DropTable(
                name: "Prepaid");

            migrationBuilder.RenameColumn(
                name: "id_group_status",
                table: "Group_Status",
                newName: "id_group_type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 5, 17, 10, 5, 14, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 6, 11, 22, 5, 329, DateTimeKind.Local));
        }
    }
}
