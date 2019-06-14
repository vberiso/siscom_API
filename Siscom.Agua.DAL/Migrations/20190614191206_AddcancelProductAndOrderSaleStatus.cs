using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddcancelProductAndOrderSaleStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 14, 14, 12, 5, 417, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 7, 13, 28, 15, 544, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "number_period",
                table: "Debt_Detail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "old_value",
                table: "Debt_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "recharges_date",
                table: "Debt",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Cancel_Product",
                columns: table => new
                {
                    id_cancel_product = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    account = table.Column<string>(maxLength: 50, nullable: true),
                    code_concept = table.Column<string>(maxLength: 5, nullable: false),
                    name_concept = table.Column<string>(maxLength: 500, nullable: false),
                    request_date = table.Column<DateTime>(nullable: false),
                    requesterId = table.Column<string>(maxLength: 100, nullable: false),
                    authorisation_date = table.Column<DateTime>(nullable: false),
                    supervisorId = table.Column<string>(maxLength: 100, nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    motivo_cancelacion = table.Column<string>(maxLength: 500, nullable: false),
                    debtId = table.Column<int>(nullable: false),
                    orderSaleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancel_Product", x => x.id_cancel_product);
                });

            migrationBuilder.CreateTable(
                name: "order_sale_status",
                columns: table => new
                {
                    id_order_status = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_status = table.Column<string>(nullable: false),
                    order_status_date = table.Column<DateTime>(nullable: false),
                    user = table.Column<string>(maxLength: 150, nullable: false),
                    OrderSaleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_sale_status", x => x.id_order_status);
                    table.ForeignKey(
                        name: "FK_order_sale_status_Order_Sale_OrderSaleId",
                        column: x => x.OrderSaleId,
                        principalTable: "Order_Sale",
                        principalColumn: "id_order_sale",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_sale_status_OrderSaleId",
                table: "order_sale_status",
                column: "OrderSaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cancel_Product");

            migrationBuilder.DropTable(
                name: "order_sale_status");

            migrationBuilder.DropColumn(
                name: "number_period",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "old_value",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "recharges_date",
                table: "Debt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 7, 13, 28, 15, 544, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 14, 14, 12, 5, 417, DateTimeKind.Local));
        }
    }
}
