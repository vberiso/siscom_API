using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableMovimientosDebt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 15, 16, 44, 54, 89, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 14, 13, 41, 49, 52, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "MovimientosDebts",
                columns: table => new
                {
                    id_movimientos_debt = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DebtIdFather = table.Column<int>(nullable: false),
                    StatusOrigin = table.Column<string>(nullable: true),
                    StatusResult = table.Column<string>(nullable: true),
                    DebtIdSon = table.Column<int>(nullable: false),
                    StatusDebtIdSon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosDebts", x => x.id_movimientos_debt);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosDebts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 14, 13, 41, 49, 52, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 15, 16, 44, 54, 89, DateTimeKind.Local));
        }
    }
}
