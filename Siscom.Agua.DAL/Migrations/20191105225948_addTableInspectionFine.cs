using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableInspectionFine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 5, 16, 59, 47, 872, DateTimeKind.Local).AddTicks(127),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 1, 15, 14, 55, 94, DateTimeKind.Local).AddTicks(2950));

            migrationBuilder.CreateTable(
                name: "inspection_fine",
                columns: table => new
                {
                    id_inspection_fine = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: true),
                    from = table.Column<int>(nullable: false),
                    until = table.Column<int>(nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    have_tax = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_fine", x => x.id_inspection_fine);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inspection_fine");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 1, 15, 14, 55, 94, DateTimeKind.Local).AddTicks(2950),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 5, 16, 59, 47, 872, DateTimeKind.Local).AddTicks(127));
        }
    }
}
