using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTablePostalCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 8, 14, 0, 0, 4, DateTimeKind.Local).AddTicks(8676),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 1, 11, 16, 32, 297, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Postal_Mx",
                columns: table => new
                {
                    IdPostalmx = table.Column<int>(nullable: false),
                    codigo = table.Column<string>(maxLength: 5, nullable: true),
                    asenta = table.Column<string>(maxLength: 150, nullable: true),
                    tipo_asenta = table.Column<string>(maxLength: 150, nullable: true),
                    municipio = table.Column<string>(maxLength: 150, nullable: true),
                    estado = table.Column<string>(maxLength: 150, nullable: true),
                    ciudad = table.Column<string>(maxLength: 150, nullable: true),
                    cp = table.Column<int>(maxLength: 6, nullable: false),
                    cod_estado = table.Column<int>(maxLength: 2, nullable: false),
                    oficina = table.Column<int>(maxLength: 5, nullable: false),
                    cod_cp = table.Column<int>(maxLength: 2, nullable: false),
                    cod_tipo_asenta = table.Column<int>(maxLength: 2, nullable: false),
                    cod_municipio = table.Column<int>(maxLength: 3, nullable: false),
                    id_asenta_cpcons = table.Column<int>(maxLength: 4, nullable: false),
                    zona = table.Column<string>(maxLength: 20, nullable: true),
                    cve_ciudad = table.Column<int>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postal_Mx", x => x.IdPostalmx);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Postal_Mx");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 1, 11, 16, 32, 297, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 8, 14, 0, 0, 4, DateTimeKind.Local).AddTicks(8676));
        }
    }
}
