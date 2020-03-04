using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ALterTableDebtCampaignModifyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DebtCampaign_AgreementId",
                table: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 27, 18, 3, 41, 553, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 27, 16, 36, 50, 147, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_DebtCampaign_AgreementId",
                table: "DebtCampaign",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DebtCampaign_AgreementId",
                table: "DebtCampaign");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 27, 16, 36, 50, 147, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 27, 18, 3, 41, 553, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_DebtCampaign_AgreementId",
                table: "DebtCampaign",
                column: "AgreementId",
                unique: true);
        }
    }
}
