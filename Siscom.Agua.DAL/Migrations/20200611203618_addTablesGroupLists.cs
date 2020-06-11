using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablesGroupLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 11, 15, 36, 17, 600, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 3, 13, 43, 35, 8, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "GroupLists",
                columns: table => new
                {
                    id_group_lists = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLists", x => x.id_group_lists);
                });

            migrationBuilder.CreateTable(
                name: "Lists",
                columns: table => new
                {
                    id_lists = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    descripction = table.Column<string>(maxLength: 50, nullable: true),
                    value_number = table.Column<decimal>(nullable: false),
                    value_text = table.Column<string>(nullable: true),
                    associated_type = table.Column<string>(maxLength: 5, nullable: true),
                    is_active = table.Column<bool>(nullable: false),
                    GroupListsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lists", x => x.id_lists);
                    table.ForeignKey(
                        name: "FK_Lists_GroupLists_GroupListsId",
                        column: x => x.GroupListsId,
                        principalTable: "GroupLists",
                        principalColumn: "id_group_lists",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lists_GroupListsId",
                table: "Lists",
                column: "GroupListsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lists");

            migrationBuilder.DropTable(
                name: "GroupLists");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 3, 13, 43, 35, 8, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 11, 15, 36, 17, 600, DateTimeKind.Local));
        }
    }
}
