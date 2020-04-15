using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldsToPhotoOrderWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "blob_photo",
                table: "PhotosOrderWork",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "PhotosOrderWork",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 14, 22, 12, 40, 281, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 5, 16, 6, 33, 884, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blob_photo",
                table: "PhotosOrderWork");

            migrationBuilder.DropColumn(
                name: "type",
                table: "PhotosOrderWork");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 5, 16, 6, 33, 884, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 14, 22, 12, 40, 281, DateTimeKind.Local));
        }
    }
}
