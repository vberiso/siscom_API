using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTypeIntakeAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 20, 12, 14, 30, 958, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 19, 12, 37, 37, 564, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "TypeIntakeId",
                table: "Agreement",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeIntakeId",
                table: "Agreement",
                column: "TypeIntakeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Type_Intake_TypeIntakeId",
                table: "Agreement",
                column: "TypeIntakeId",
                principalTable: "Type_Intake",
                principalColumn: "id_type_intake",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Type_Intake_TypeIntakeId",
                table: "Agreement");

            migrationBuilder.DropIndex(
                name: "IX_Agreement_TypeIntakeId",
                table: "Agreement");

            migrationBuilder.DropColumn(
                name: "TypeIntakeId",
                table: "Agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 19, 12, 37, 37, 564, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 20, 12, 14, 30, 958, DateTimeKind.Local));
        }
    }
}
