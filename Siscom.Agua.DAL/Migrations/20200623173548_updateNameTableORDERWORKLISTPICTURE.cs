using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class updateNameTableORDERWORKLISTPICTURE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderWorkListPictures_order_work_list_OrderWorkListId",
                table: "OrderWorkListPictures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderWorkListPictures",
                table: "OrderWorkListPictures");

            migrationBuilder.RenameTable(
                name: "OrderWorkListPictures",
                newName: "order_work_list_picture");

            migrationBuilder.RenameIndex(
                name: "IX_OrderWorkListPictures_OrderWorkListId",
                table: "order_work_list_picture",
                newName: "IX_order_work_list_picture_OrderWorkListId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 23, 12, 35, 47, 348, DateTimeKind.Local).AddTicks(5629),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 23, 12, 31, 55, 834, DateTimeKind.Local).AddTicks(4980));

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_work_list_picture",
                table: "order_work_list_picture",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_work_list_picture_order_work_list_OrderWorkListId",
                table: "order_work_list_picture",
                column: "OrderWorkListId",
                principalTable: "order_work_list",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_work_list_picture_order_work_list_OrderWorkListId",
                table: "order_work_list_picture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_work_list_picture",
                table: "order_work_list_picture");

            migrationBuilder.RenameTable(
                name: "order_work_list_picture",
                newName: "OrderWorkListPictures");

            migrationBuilder.RenameIndex(
                name: "IX_order_work_list_picture_OrderWorkListId",
                table: "OrderWorkListPictures",
                newName: "IX_OrderWorkListPictures_OrderWorkListId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 23, 12, 31, 55, 834, DateTimeKind.Local).AddTicks(4980),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 23, 12, 35, 47, 348, DateTimeKind.Local).AddTicks(5629));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderWorkListPictures",
                table: "OrderWorkListPictures",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderWorkListPictures_order_work_list_OrderWorkListId",
                table: "OrderWorkListPictures",
                column: "OrderWorkListId",
                principalTable: "order_work_list",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
