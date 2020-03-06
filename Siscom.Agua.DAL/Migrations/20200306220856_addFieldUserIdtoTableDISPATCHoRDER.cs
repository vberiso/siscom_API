using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldUserIdtoTableDISPATCHoRDER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 6, 16, 8, 55, 130, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 5, 17, 0, 21, 217, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "DispatchOrder",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "DispatchOrder");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 5, 17, 0, 21, 217, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 6, 16, 8, 55, 130, DateTimeKind.Local));
        }
    }
}
