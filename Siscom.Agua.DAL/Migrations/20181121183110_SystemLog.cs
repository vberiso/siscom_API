using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class SystemLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_date",
                table: "Folio");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 12, 31, 10, 6, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "System_Log",
                columns: table => new
                {
                    id_system_log = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date_log = table.Column<DateTime>(nullable: false),
                    controller = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    parameter = table.Column<string>(nullable: false),
                    action = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Log", x => x.id_system_log);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "System_Log");

            migrationBuilder.DropColumn(
                name: "date_current",
                table: "Folio");

            migrationBuilder.AddColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 11, 24, 19, 194, DateTimeKind.Local));
        }
    }
}
