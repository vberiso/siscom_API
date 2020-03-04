using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTableDebtCampaignFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 28, 15, 38, 22, 122, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 27, 18, 3, 41, 553, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "debt_campaign_files",
                columns: table => new
                {
                    id_debt_campaign_files = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>(nullable: true),
                    user_name = table.Column<string>(nullable: true),
                    generation_date = table.Column<DateTime>(nullable: false),
                    file_name = table.Column<string>(maxLength: 200, nullable: true),
                    pdf = table.Column<byte[]>(nullable: true),
                    total_records = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debt_campaign_files", x => x.id_debt_campaign_files);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "debt_campaign_files");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 27, 18, 3, 41, 553, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 28, 15, 38, 22, 122, DateTimeKind.Local));
        }
    }
}
