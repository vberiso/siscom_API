using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class SAT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unit_measurement",
                table: "Tariff_Product");

            migrationBuilder.DropColumn(
                name: "unit_measurement",
                table: "Tariff");

            migrationBuilder.AlterColumn<bool>(
                name: "have_tax_receipt",
                table: "Payment",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 26, 15, 32, 8, 352, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 26, 11, 23, 26, 56, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Product_Param",
                columns: table => new
                {
                    id_product_param = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 20, nullable: false),
                    name_concept = table.Column<string>(maxLength: 500, nullable: false),
                    unit_measurement = table.Column<string>(maxLength: 10, nullable: false),
                    is_active = table.Column<int>(nullable: false, defaultValue: 1),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Param", x => x.id_product_param);
                    table.ForeignKey(
                        name: "FK_Product_Param_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "id_product",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service_Param",
                columns: table => new
                {
                    id_service_param = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 20, nullable: false),
                    name_concept = table.Column<string>(maxLength: 500, nullable: false),
                    unit_measurement = table.Column<string>(maxLength: 10, nullable: false),
                    is_active = table.Column<int>(nullable: false, defaultValue: 1),
                    ServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service_Param", x => x.id_service_param);
                    table.ForeignKey(
                        name: "FK_Service_Param_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_Param_ProductId",
                table: "Product_Param",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Param_ServiceId",
                table: "Service_Param",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product_Param");

            migrationBuilder.DropTable(
                name: "Service_Param");

            migrationBuilder.AddColumn<string>(
                name: "unit_measurement",
                table: "Tariff_Product",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "unit_measurement",
                table: "Tariff",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "have_tax_receipt",
                table: "Payment",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 26, 11, 23, 26, 56, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 26, 15, 32, 8, 352, DateTimeKind.Local));
        }
    }
}
