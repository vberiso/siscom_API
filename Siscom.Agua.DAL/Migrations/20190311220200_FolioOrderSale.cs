using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class FolioOrderSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 11, 16, 1, 59, 504, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 8, 18, 59, 41, 919, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "FolioOrderSale",
                columns: table => new
                {
                    id_folio_order_sale = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prefix = table.Column<string>(nullable: true),
                    secuential = table.Column<int>(nullable: false),
                    suffixes = table.Column<string>(nullable: true),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    type = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolioOrderSale", x => x.id_folio_order_sale);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolioOrderSale");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 8, 18, 59, 41, 919, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 11, 16, 1, 59, 504, DateTimeKind.Local));
        }
    }
}
