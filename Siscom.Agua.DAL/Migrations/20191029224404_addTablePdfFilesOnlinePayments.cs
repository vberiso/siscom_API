using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablePdfFilesOnlinePayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 29, 16, 44, 3, 899, DateTimeKind.Local).AddTicks(3093),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 28, 18, 7, 30, 907, DateTimeKind.Local).AddTicks(557));

            migrationBuilder.CreateTable(
                name: "online_payment_file",
                columns: table => new
                {
                    online_payment_file_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date_generated = table.Column<DateTime>(nullable: false),
                    id_agreement = table.Column<int>(nullable: false),
                    account = table.Column<string>(nullable: true),
                    token = table.Column<string>(nullable: true),
                    folio = table.Column<string>(nullable: true),
                    month = table.Column<int>(nullable: false),
                    year = table.Column<int>(nullable: false),
                    pdf_invoce = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_online_payment_file", x => x.online_payment_file_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "online_payment_file");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 28, 18, 7, 30, 907, DateTimeKind.Local).AddTicks(557),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 29, 16, 44, 3, 899, DateTimeKind.Local).AddTicks(3093));
        }
    }
}
