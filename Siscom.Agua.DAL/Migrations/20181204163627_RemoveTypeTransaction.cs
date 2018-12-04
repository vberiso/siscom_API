using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class RemoveTypeTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Type_Transaction_TypeTransactionId",
                table: "Agreement");

            migrationBuilder.DropIndex(
                name: "IX_Agreement_TypeTransactionId",
                table: "Agreement");

            migrationBuilder.DropColumn(
                name: "TypeTransactionId",
                table: "Agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 4, 10, 36, 26, 666, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 3, 17, 51, 43, 101, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 3, 17, 51, 43, 101, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 4, 10, 36, 26, 666, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "TypeTransactionId",
                table: "Agreement",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeTransactionId",
                table: "Agreement",
                column: "TypeTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Type_Transaction_TypeTransactionId",
                table: "Agreement",
                column: "TypeTransactionId",
                principalTable: "Type_Transaction",
                principalColumn: "id_type_transaction",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
