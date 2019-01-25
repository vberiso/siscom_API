using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Catalogue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 25, 12, 36, 27, 881, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 23, 14, 56, 49, 220, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Group_Catalogue",
                columns: table => new
                {
                    id_group_catalogue = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Catalogue", x => x.id_group_catalogue);
                });

            migrationBuilder.CreateTable(
                name: "Catalogue",
                columns: table => new
                {
                    id_catalogue = table.Column<string>(nullable: false),
                    value = table.Column<string>(maxLength: 30, nullable: false),
                    GroupCatalogueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogue", x => x.id_catalogue);
                    table.ForeignKey(
                        name: "FK_Catalogue_Group_Catalogue_GroupCatalogueId",
                        column: x => x.GroupCatalogueId,
                        principalTable: "Group_Catalogue",
                        principalColumn: "id_group_catalogue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalogue_GroupCatalogueId",
                table: "Catalogue",
                column: "GroupCatalogueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalogue");

            migrationBuilder.DropTable(
                name: "Group_Catalogue");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 23, 14, 56, 49, 220, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 25, 12, 36, 27, 881, DateTimeKind.Local));
        }
    }
}
