using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableDebtAnnual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 14, 13, 25, 10, 284, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 12, 11, 25, 42, 815, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "debt_annual",
                columns: table => new
                {
                    id_debt_annual = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    debit_date = table.Column<DateTime>(nullable: false),
                    from_date = table.Column<DateTime>(type: "date", nullable: false),
                    until_date = table.Column<DateTime>(type: "date", nullable: false),
                    type_intake = table.Column<string>(maxLength: 50, nullable: true),
                    type_service = table.Column<string>(nullable: true),
                    year = table.Column<short>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: true),
                    sequential = table.Column<int>(nullable: false),
                    code_concept = table.Column<string>(maxLength: 5, nullable: true),
                    name_concept = table.Column<string>(maxLength: 500, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    debt_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debt_annual", x => x.id_debt_annual);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "debt_annual");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 12, 11, 25, 42, 815, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 14, 13, 25, 10, 284, DateTimeKind.Local));
        }
    }
}
