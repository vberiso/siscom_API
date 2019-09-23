using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class changeRelationOrderWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_work_AgreementId",
                table: "Order_work");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 23, 17, 33, 40, 957, DateTimeKind.Local).AddTicks(1266),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 17, 15, 55, 0, 216, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_Order_work_AgreementId",
                table: "Order_work",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_work_AgreementId",
                table: "Order_work");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 17, 15, 55, 0, 216, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 23, 17, 33, 40, 957, DateTimeKind.Local).AddTicks(1266));

            migrationBuilder.CreateIndex(
                name: "IX_Order_work_AgreementId",
                table: "Order_work",
                column: "AgreementId",
                unique: true);
        }
    }
}
