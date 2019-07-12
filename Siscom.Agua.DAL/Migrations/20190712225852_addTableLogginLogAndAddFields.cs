using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableLogginLogAndAddFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderSaleDetailsId",
                table: "Order_Sale_Discount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 12, 17, 58, 51, 652, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 10, 14, 48, 2, 96, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "DebtDetailId",
                table: "Debt_Discount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LogginLog",
                columns: table => new
                {
                    id_loggingLog = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Terminal = table.Column<string>(maxLength: 30, nullable: true),
                    User = table.Column<string>(maxLength: 30, nullable: true),
                    VersionSiscom = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogginLog", x => x.id_loggingLog);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogginLog");

            migrationBuilder.DropColumn(
                name: "OrderSaleDetailsId",
                table: "Order_Sale_Discount");

            migrationBuilder.DropColumn(
                name: "DebtDetailId",
                table: "Debt_Discount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 10, 14, 48, 2, 96, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 12, 17, 58, 51, 652, DateTimeKind.Local));
        }
    }
}
