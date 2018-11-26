using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementLogFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 26, 12, 19, 1, 42, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 26, 12, 11, 19, 813, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "AgreementId",
                table: "Agreement_Log",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Log_AgreementId",
                table: "Agreement_Log",
                column: "AgreementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Log_Agreement_AgreementId",
                table: "Agreement_Log",
                column: "AgreementId",
                principalTable: "Agreement",
                principalColumn: "id_agreement",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Log_Agreement_AgreementId",
                table: "Agreement_Log");

            migrationBuilder.DropIndex(
                name: "IX_Agreement_Log_AgreementId",
                table: "Agreement_Log");

            migrationBuilder.DropColumn(
                name: "AgreementId",
                table: "Agreement_Log");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 26, 12, 11, 19, 813, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 26, 12, 19, 1, 42, DateTimeKind.Local));
        }
    }
}
