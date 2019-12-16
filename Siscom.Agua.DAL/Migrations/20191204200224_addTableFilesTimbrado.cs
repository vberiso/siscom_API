using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableFilesTimbrado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 12, 4, 14, 2, 22, 240, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 12, 27, 17, 23, 29, 932, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Files_Timbrado",
                columns: table => new
                {
                    id_Files_Timbrado = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    path_file = table.Column<string>(maxLength: 50, nullable: false),
                    name_file = table.Column<string>(maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: true),
                    end_date = table.Column<DateTime>(nullable: true),
                    pass_key = table.Column<string>(maxLength: 50, nullable: true),
                    certificate_number = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files_Timbrado", x => x.id_Files_Timbrado);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files_Timbrado");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 12, 27, 17, 23, 29, 932, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 12, 4, 14, 2, 22, 240, DateTimeKind.Local));
        }
    }
}
