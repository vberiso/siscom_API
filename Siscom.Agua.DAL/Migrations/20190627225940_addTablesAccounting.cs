using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablesAccounting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 27, 17, 59, 39, 851, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 27, 15, 33, 18, 161, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "accounting_payment",
                columns: table => new
                {
                    id_accounting_payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_payment = table.Column<int>(nullable: false),
                    code_sac = table.Column<int>(nullable: false),
                    procedure_code = table.Column<int>(nullable: false),
                    secuential = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    amount_tax = table.Column<decimal>(nullable: false),
                    desciption_code = table.Column<string>(maxLength: 500, nullable: true),
                    request_date = table.Column<DateTime>(nullable: false),
                    is_dispatched = table.Column<int>(nullable: false),
                    movement_type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_payment", x => x.id_accounting_payment);
                });

            migrationBuilder.CreateTable(
                name: "accounting_status",
                columns: table => new
                {
                    id_accounting_status = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    request_date = table.Column<DateTime>(nullable: false),
                    POI_error = table.Column<int>(nullable: false),
                    POC_mensage_error = table.Column<string>(maxLength: 500, nullable: true),
                    accounting_code_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_status", x => x.id_accounting_status);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_payment");

            migrationBuilder.DropTable(
                name: "accounting_status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 27, 15, 33, 18, 161, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 27, 17, 59, 39, 851, DateTimeKind.Local));
        }
    }
}
