using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableCondonationCampaing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 11, 11, 41, 9, 862, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 8, 13, 10, 52, 373, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "CondonationCampaings",
                columns: table => new
                {
                    id_condonation_campaign = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    alias = table.Column<string>(maxLength: 200, nullable: true),
                    tipes = table.Column<string>(maxLength: 100, nullable: true),
                    codes = table.Column<string>(maxLength: 200, nullable: true),
                    percentage = table.Column<short>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondonationCampaings", x => x.id_condonation_campaign);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CondonationCampaings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 8, 13, 10, 52, 373, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 11, 11, 41, 9, 862, DateTimeKind.Local));
        }
    }
}
