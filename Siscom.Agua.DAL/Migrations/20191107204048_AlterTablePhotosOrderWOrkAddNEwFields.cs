using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterTablePhotosOrderWOrkAddNEwFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_photo",
                table: "PhotosOrderWork",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_file",
                table: "PhotosOrderWork",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "size",
                table: "PhotosOrderWork",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 7, 14, 40, 47, 590, DateTimeKind.Local).AddTicks(7544),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 7, 11, 22, 7, 110, DateTimeKind.Local).AddTicks(3073));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name_file",
                table: "PhotosOrderWork");

            migrationBuilder.DropColumn(
                name: "size",
                table: "PhotosOrderWork");

            migrationBuilder.AlterColumn<string>(
                name: "date_photo",
                table: "PhotosOrderWork",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 7, 11, 22, 7, 110, DateTimeKind.Local).AddTicks(3073),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 7, 14, 40, 47, 590, DateTimeKind.Local).AddTicks(7544));
        }
    }
}
