using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableGenericFolios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 2, 2, 22, 50, 39, 72, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 1, 29, 17, 9, 21, 292, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "generic_folios",
                columns: table => new
                {
                    id_generic_folio = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prefix = table.Column<string>(maxLength: 3, nullable: true),
                    secuential = table.Column<int>(nullable: false),
                    suffixes = table.Column<string>(maxLength: 3, nullable: true),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    type = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_generic_folios", x => x.id_generic_folio);
                });

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 2, 2, 22, 50, 39, 123, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "generic_folios");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2021, 1, 29, 17, 9, 21, 292, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2021, 2, 2, 22, 50, 39, 72, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2021, 1, 29, 17, 9, 21, 342, DateTimeKind.Local));
        }
    }
}
