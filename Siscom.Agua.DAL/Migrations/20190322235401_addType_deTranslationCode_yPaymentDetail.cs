using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addType_deTranslationCode_yPaymentDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Translation_Code_id_group_translation_id_product",
                table: "Translation_Code");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translation_Code",
                table: "Translation_Code");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "Translation_Code",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Translation_Code",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Payment_Detail",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 22, 17, 54, 1, 205, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 22, 13, 29, 3, 800, DateTimeKind.Local));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Translation_Code_id_group_translation_id_product_type",
                table: "Translation_Code",
                columns: new[] { "id_group_translation", "id_product", "type" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translation_Code",
                table: "Translation_Code",
                columns: new[] { "id_product", "id_group_translation", "type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Translation_Code_id_group_translation_id_product_type",
                table: "Translation_Code");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translation_Code",
                table: "Translation_Code");

            migrationBuilder.DropColumn(
                name: "type",
                table: "Translation_Code");

            migrationBuilder.DropColumn(
                name: "type",
                table: "Payment_Detail");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "Translation_Code",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 22, 13, 29, 3, 800, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 22, 17, 54, 1, 205, DateTimeKind.Local));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Translation_Code_id_group_translation_id_product",
                table: "Translation_Code",
                columns: new[] { "id_group_translation", "id_product" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translation_Code",
                table: "Translation_Code",
                columns: new[] { "id_product", "id_group_translation" });
        }
    }
}
