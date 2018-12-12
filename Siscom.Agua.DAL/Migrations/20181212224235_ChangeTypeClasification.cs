using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeTypeClasification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Type_Classification_TypeClassificationId",
                table: "Agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 12, 16, 42, 34, 226, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 11, 13, 4, 14, 18, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "TypeClassificationId",
                table: "Agreement",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Type_Classification_TypeClassificationId",
                table: "Agreement",
                column: "TypeClassificationId",
                principalTable: "Type_Classification",
                principalColumn: "id_type_classification",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Type_Classification_TypeClassificationId",
                table: "Agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 11, 13, 4, 14, 18, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 12, 16, 42, 34, 226, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "TypeClassificationId",
                table: "Agreement",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Type_Classification_TypeClassificationId",
                table: "Agreement",
                column: "TypeClassificationId",
                principalTable: "Type_Classification",
                principalColumn: "id_type_classification",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
