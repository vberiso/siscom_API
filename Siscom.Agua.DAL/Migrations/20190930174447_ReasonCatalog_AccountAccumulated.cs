using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ReasonCatalog_AccountAccumulated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 30, 12, 44, 46, 446, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 23, 17, 33, 40, 957, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "AccountsAccumulated",
                columns: table => new
                {
                    id_account_accumulated = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    accumulated = table.Column<int>(nullable: false),
                    year = table.Column<int>(nullable: false),
                    mes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsAccumulated", x => x.id_account_accumulated);
                });

            migrationBuilder.CreateTable(
                name: "ReasonCatalog",
                columns: table => new
                {
                    id_reasoncatalog = table.Column<int>(name: "id_reason-catalog", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonCatalog", x => x.id_reasoncatalog);
                });

            migrationBuilder.CreateTable(
                name: "OrderWorkReasonCatalog",
                columns: table => new
                {
                    OrderWorkId = table.Column<int>(nullable: false),
                    ReasonCatalogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderWorkReasonCatalog", x => new { x.OrderWorkId, x.ReasonCatalogId });
                    table.ForeignKey(
                        name: "FK_OrderWorkReasonCatalog_Order_work_OrderWorkId",
                        column: x => x.OrderWorkId,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderWorkReasonCatalog_ReasonCatalog_ReasonCatalogId",
                        column: x => x.ReasonCatalogId,
                        principalTable: "ReasonCatalog",
                        principalColumn: "id_reason-catalog",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderWorkReasonCatalog_ReasonCatalogId",
                table: "OrderWorkReasonCatalog",
                column: "ReasonCatalogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountsAccumulated");

            migrationBuilder.DropTable(
                name: "OrderWorkReasonCatalog");

            migrationBuilder.DropTable(
                name: "ReasonCatalog");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 23, 17, 33, 40, 957, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 30, 12, 44, 46, 446, DateTimeKind.Local));
        }
    }
}
