using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class OrderSaleDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "min",
                table: "Breach_List");

            migrationBuilder.AlterColumn<string>(
                name: "code_concept",
                table: "Order_Sale_Detail",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 5, 16, 9, 6, 375, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 1, 15, 4, 48, 365, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "is_variable",
                table: "Discount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "max_times_factor",
                table: "Breach_List",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "min_times_factor",
                table: "Breach_List",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "Discount_Campaign",
                columns: table => new
                {
                    id_discount_campaign = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    percentage = table.Column<short>(nullable: false),
                    is_variable = table.Column<bool>(nullable: false, defaultValue: false),
                    profile = table.Column<bool>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount_Campaign", x => x.id_discount_campaign);
                });

            migrationBuilder.CreateTable(
                name: "Order_Sale_Discount",
                columns: table => new
                {
                    id_order_sale_discount = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 10, nullable: false),
                    name_concept = table.Column<string>(maxLength: 500, nullable: false),
                    original_amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    discount_percentage = table.Column<short>(nullable: false),
                    OrderSaleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Sale_Discount", x => x.id_order_sale_discount);
                    table.ForeignKey(
                        name: "FK_Order_Sale_Discount_Order_Sale_OrderSaleId",
                        column: x => x.OrderSaleId,
                        principalTable: "Order_Sale",
                        principalColumn: "id_order_sale",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Sale_Discount_OrderSaleId",
                table: "Order_Sale_Discount",
                column: "OrderSaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discount_Campaign");

            migrationBuilder.DropTable(
                name: "Order_Sale_Discount");

            migrationBuilder.DropColumn(
                name: "is_variable",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "max_times_factor",
                table: "Breach_List");

            migrationBuilder.DropColumn(
                name: "min_times_factor",
                table: "Breach_List");

            migrationBuilder.AlterColumn<string>(
                name: "code_concept",
                table: "Order_Sale_Detail",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 1, 15, 4, 48, 365, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 5, 16, 9, 6, 375, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "min",
                table: "Breach_List",
                nullable: false,
                defaultValue: 0);
        }
    }
}
