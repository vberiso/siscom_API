using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldsPartialPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "partial_payment",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "identification_card",
                table: "partial_payment",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "identification_number",
                table: "partial_payment",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "partial_payment",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "signature_name",
                table: "partial_payment",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 31, 17, 35, 3, 621, DateTimeKind.Local).AddTicks(6407),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 30, 18, 5, 30, 876, DateTimeKind.Local).AddTicks(1104));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "partial_payment");

            migrationBuilder.DropColumn(
                name: "identification_card",
                table: "partial_payment");

            migrationBuilder.DropColumn(
                name: "identification_number",
                table: "partial_payment");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "partial_payment");

            migrationBuilder.DropColumn(
                name: "signature_name",
                table: "partial_payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 30, 18, 5, 30, 876, DateTimeKind.Local).AddTicks(1104),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 31, 17, 35, 3, 621, DateTimeKind.Local).AddTicks(6407));
        }
    }
}
