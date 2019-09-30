using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableSkipArticles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name_concept",
                table: "Order_Sale_Detail",
                maxLength: 800,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 27, 14, 45, 10, 233, DateTimeKind.Local).AddTicks(1064),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 23, 17, 33, 40, 957, DateTimeKind.Local).AddTicks(1266));

            migrationBuilder.CreateTable(
                name: "Skip_Articles",
                columns: table => new
                {
                    id_skip_articles = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    article = table.Column<string>(maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skip_Articles", x => x.id_skip_articles);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skip_Articles");

            migrationBuilder.AlterColumn<string>(
                name: "name_concept",
                table: "Order_Sale_Detail",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 800);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 23, 17, 33, 40, 957, DateTimeKind.Local).AddTicks(1266),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 27, 14, 45, 10, 233, DateTimeKind.Local).AddTicks(1064));
        }
    }
}
