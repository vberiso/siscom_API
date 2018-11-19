using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DeleteTableType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 19, 12, 49, 14, 83, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 19, 12, 37, 37, 564, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "type_address",
                table: "Address",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(byte));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 19, 12, 37, 37, 564, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 19, 12, 49, 14, 83, DateTimeKind.Local));

            migrationBuilder.AlterColumn<byte>(
                name: "type_address",
                table: "Address",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    id_type = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    GroupTypeId = table.Column<int>(nullable: true),
                    name = table.Column<string>(maxLength: 5, nullable: false)
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
    }
}
