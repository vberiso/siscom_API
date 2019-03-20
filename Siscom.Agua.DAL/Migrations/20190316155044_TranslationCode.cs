using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TranslationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 16, 9, 50, 43, 918, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 15, 9, 31, 5, 488, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Group_Translation_Code",
                columns: table => new
                {
                    id_group_translation_codes = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Translation_Code", x => x.id_group_translation_codes);
                });

            migrationBuilder.CreateTable(
                name: "Translation_Code",
                columns: table => new
                {
                    id_product = table.Column<int>(nullable: false),
                    id_group_translation = table.Column<int>(nullable: false),
                    code = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation_Code", x => new { x.id_product, x.id_group_translation });
                    table.UniqueConstraint("AK_Translation_Code_id_group_translation_id_product", x => new { x.id_group_translation, x.id_product });
                    table.ForeignKey(
                        name: "FK_Translation_Code_Group_Translation_Code_id_group_translation",
                        column: x => x.id_group_translation,
                        principalTable: "Group_Translation_Code",
                        principalColumn: "id_group_translation_codes",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translation_Code");

            migrationBuilder.DropTable(
                name: "Group_Translation_Code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 15, 9, 31, 5, 488, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 16, 9, 50, 43, 918, DateTimeKind.Local));
        }
    }
}
