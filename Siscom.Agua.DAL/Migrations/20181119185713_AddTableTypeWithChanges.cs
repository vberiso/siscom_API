using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTableTypeWithChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 19, 12, 57, 13, 269, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 19, 12, 49, 14, 83, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    id_type = table.Column<string>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    GroupTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.id_type);
                    table.ForeignKey(
                        name: "FK_Type_Group_Type_GroupTypeId",
                        column: x => x.GroupTypeId,
                        principalTable: "Group_Type",
                        principalColumn: "id_group_type",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Type_GroupTypeId",
                table: "Type",
                column: "GroupTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 19, 12, 49, 14, 83, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 19, 12, 57, 13, 269, DateTimeKind.Local));
        }
    }
}
