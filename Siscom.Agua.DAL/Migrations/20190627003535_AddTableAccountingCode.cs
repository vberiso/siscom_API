using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTableAccountingCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 26, 19, 35, 34, 601, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 21, 14, 54, 5, 662, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "accounting_code",
                columns: table => new
                {
                    id_accounting_code = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    origin = table.Column<string>(nullable: false),
                    code_concept = table.Column<int>(nullable: false),
                    name_concept = table.Column<string>(maxLength: 400, nullable: true),
                    id_divition = table.Column<short>(nullable: false),
                    name_divition = table.Column<string>(maxLength: 200, nullable: true),
                    code_sac = table.Column<int>(nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_code", x => x.id_accounting_code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 21, 14, 54, 5, 662, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 26, 19, 35, 34, 601, DateTimeKind.Local));
        }
    }
}
