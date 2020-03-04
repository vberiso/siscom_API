using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterTableDebtCampaingFilesALterFieldFolio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 3, 11, 45, 24, 323, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 3, 11, 37, 6, 384, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "folio",
                table: "debt_campaign_files",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 3, 11, 37, 6, 384, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 3, 11, 45, 24, 323, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "folio",
                table: "debt_campaign_files",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
