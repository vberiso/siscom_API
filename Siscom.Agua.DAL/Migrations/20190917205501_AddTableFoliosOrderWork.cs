using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTableFoliosOrderWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 17, 15, 55, 0, 216, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 13, 16, 55, 8, 265, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Folio_order_work",
                columns: table => new
                {
                    id_folio_order_work = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prefix = table.Column<string>(maxLength: 3, nullable: true),
                    secuential = table.Column<int>(nullable: false),
                    suffixes = table.Column<string>(maxLength: 3, nullable: true),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    type = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folio_order_work", x => x.id_folio_order_work);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Folio_order_work");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 13, 16, 55, 8, 265, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 17, 15, 55, 0, 216, DateTimeKind.Local));
        }
    }
}
