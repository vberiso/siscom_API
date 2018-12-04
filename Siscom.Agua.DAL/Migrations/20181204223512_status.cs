using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Group_Status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 4, 16, 35, 12, 523, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 4, 16, 5, 0, 728, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 4, 16, 5, 0, 728, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 4, 16, 35, 12, 523, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Group_Status",
                columns: table => new
                {
                    id_group_type = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Status", x => x.id_group_type);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    id_status = table.Column<string>(nullable: false),
                    GroupStatusId = table.Column<int>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => new { x.id_status, x.GroupStatusId });
                    table.UniqueConstraint("AK_Status_id_status", x => x.id_status);
                    table.ForeignKey(
                        name: "FK_Status_Group_Status_GroupStatusId",
                        column: x => x.GroupStatusId,
                        principalTable: "Group_Status",
                        principalColumn: "id_group_type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Status_GroupStatusId",
                table: "Status",
                column: "GroupStatusId");
        }
    }
}
