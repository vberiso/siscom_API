using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ALterTableDebtCampaignModigyFIeldIsInvitation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 4, 17, 57, 39, 616, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 3, 13, 1, 5, 311, DateTimeKind.Local));

            migrationBuilder.AlterColumn<bool>(
                name: "IsInvitation",
                table: "debt_campaign_files",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 3, 13, 1, 5, 311, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 4, 17, 57, 39, 616, DateTimeKind.Local));

            migrationBuilder.AlterColumn<bool>(
                name: "IsInvitation",
                table: "debt_campaign_files",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));
        }
    }
}
