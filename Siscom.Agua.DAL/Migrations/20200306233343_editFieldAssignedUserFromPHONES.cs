using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class editFieldAssignedUserFromPHONES : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "assigned_user",
                table: "phone",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 6, 17, 33, 41, 844, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 6, 16, 8, 55, 130, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "assigned_user",
                table: "phone",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 6, 16, 8, 55, 130, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 6, 17, 33, 41, 844, DateTimeKind.Local));
        }
    }
}
