using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableFolioAccountStatement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 30, 18, 5, 30, 876, DateTimeKind.Local).AddTicks(1104),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 29, 16, 44, 3, 899, DateTimeKind.Local).AddTicks(3093));

            migrationBuilder.CreateTable(
                name: "FolioAccountStatements",
                columns: table => new
                {
                    id_folio_account_statement = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prefix = table.Column<string>(maxLength: 3, nullable: true),
                    secuential = table.Column<int>(nullable: false),
                    suffixes = table.Column<string>(maxLength: 3, nullable: true),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    type = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolioAccountStatements", x => x.id_folio_account_statement);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolioAccountStatements");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 29, 16, 44, 3, 899, DateTimeKind.Local).AddTicks(3093),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 30, 18, 5, 30, 876, DateTimeKind.Local).AddTicks(1104));
        }
    }
}
