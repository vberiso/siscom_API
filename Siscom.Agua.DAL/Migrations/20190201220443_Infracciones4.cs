using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Infracciones4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 1, 16, 4, 43, 63, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 28, 16, 41, 12, 642, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "id_solution",
                table: "Division",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Division",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "Assignment_Ticket",
                columns: table => new
                {
                    id_assignment_ticket = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date_assignment = table.Column<DateTime>(nullable: false),
                    serie = table.Column<string>(maxLength: 50, nullable: true),
                    folio = table.Column<string>(maxLength: 10, nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment_Ticket", x => x.id_assignment_ticket);
                    table.ForeignKey(
                        name: "FK_Assignment_Ticket_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breach_Article",
                columns: table => new
                {
                    id_breach_article = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    article = table.Column<string>(maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breach_Article", x => x.id_breach_article);
                });

            migrationBuilder.CreateTable(
                name: "Order_Sale",
                columns: table => new
                {
                    id_order_sale = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 30, nullable: false),
                    date_order = table.Column<DateTime>(nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    on_account = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    year = table.Column<short>(nullable: false),
                    period = table.Column<short>(nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    observation = table.Column<string>(maxLength: 250, nullable: true),
                    id_origin = table.Column<int>(nullable: false),
                    id_tax_user = table.Column<int>(nullable: false),
                    expiration_date = table.Column<DateTime>(type: "date", nullable: false),
                    DivisionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Sale", x => x.id_order_sale);
                    table.ForeignKey(
                        name: "FK_Order_Sale_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "id_division",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tax_User",
                columns: table => new
                {
                    id_tax_user = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 200, nullable: false),
                    rfc = table.Column<string>(maxLength: 17, nullable: true),
                    curp = table.Column<string>(maxLength: 18, nullable: true),
                    phone_number = table.Column<string>(maxLength: 50, nullable: false),
                    email = table.Column<string>(maxLength: 150, nullable: true),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_User", x => x.id_tax_user);
                });

            migrationBuilder.CreateTable(
                name: "Warranty",
                columns: table => new
                {
                    id_breach_warranty = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranty", x => x.id_breach_warranty);
                });

            migrationBuilder.CreateTable(
                name: "Breach_List",
                columns: table => new
                {
                    id_breach_list = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    fraction = table.Column<string>(maxLength: 40, nullable: false),
                    description = table.Column<string>(maxLength: 100, nullable: false),
                    min = table.Column<int>(nullable: false),
                    have_bonification = table.Column<bool>(nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    BreachArticleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breach_List", x => x.id_breach_list);
                    table.ForeignKey(
                        name: "FK_Breach_List_Breach_Article_BreachArticleId",
                        column: x => x.BreachArticleId,
                        principalTable: "Breach_Article",
                        principalColumn: "id_breach_article",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order_Sale_Detail",
                columns: table => new
                {
                    id_order_sale_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    quantity = table.Column<short>(nullable: false),
                    unity = table.Column<string>(maxLength: 10, nullable: false),
                    unit_price = table.Column<decimal>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    description = table.Column<string>(maxLength: 500, nullable: false),
                    code_concept = table.Column<string>(maxLength: 5, nullable: false),
                    name_concept = table.Column<string>(maxLength: 500, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    on_account = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    OrderSaleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Sale_Detail", x => x.id_order_sale_detail);
                    table.ForeignKey(
                        name: "FK_Order_Sale_Detail_Order_Sale_OrderSaleId",
                        column: x => x.OrderSaleId,
                        principalTable: "Order_Sale",
                        principalColumn: "id_order_sale",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breach",
                columns: table => new
                {
                    id_breach = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    series = table.Column<string>(maxLength: 50, nullable: false),
                    folio = table.Column<string>(maxLength: 10, nullable: false),
                    date_capture = table.Column<DateTime>(nullable: false),
                    place = table.Column<string>(maxLength: 256, nullable: false),
                    sector = table.Column<string>(maxLength: 50, nullable: true),
                    zone = table.Column<string>(maxLength: 50, nullable: true),
                    car = table.Column<string>(maxLength: 100, nullable: false),
                    type_car = table.Column<string>(maxLength: 100, nullable: false),
                    service = table.Column<string>(maxLength: 100, nullable: false),
                    color = table.Column<string>(maxLength: 100, nullable: false),
                    licenseplate = table.Column<string>(maxLength: 50, nullable: false),
                    reason = table.Column<string>(maxLength: 256, nullable: false),
                    judge = table.Column<decimal>(nullable: false),
                    date_breach = table.Column<DateTime>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    id_assignment_ticket = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    TaxUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breach", x => x.id_breach);
                    table.ForeignKey(
                        name: "FK_Breach_Tax_User_TaxUserId",
                        column: x => x.TaxUserId,
                        principalTable: "Tax_User",
                        principalColumn: "id_tax_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Breach_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tax_Address",
                columns: table => new
                {
                    id_tax_address = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    street = table.Column<string>(maxLength: 150, nullable: true),
                    outdoor = table.Column<string>(maxLength: 50, nullable: false),
                    indoor = table.Column<string>(maxLength: 50, nullable: true),
                    zip = table.Column<string>(maxLength: 5, nullable: true),
                    suburb = table.Column<string>(maxLength: 100, nullable: true),
                    town = table.Column<string>(maxLength: 30, nullable: true),
                    state = table.Column<string>(maxLength: 30, nullable: true),
                    TaxUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_Address", x => x.id_tax_address);
                    table.ForeignKey(
                        name: "FK_Tax_Address_Tax_User_TaxUserId",
                        column: x => x.TaxUserId,
                        principalTable: "Tax_User",
                        principalColumn: "id_tax_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breach_Detail",
                columns: table => new
                {
                    id_breach_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    aplication_days = table.Column<int>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    porcent_bonification = table.Column<decimal>(nullable: false),
                    bonification = table.Column<decimal>(nullable: false),
                    BreachId = table.Column<int>(nullable: false),
                    BreachListId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breach_Detail", x => x.id_breach_detail);
                    table.ForeignKey(
                        name: "FK_Breach_Detail_Breach_BreachId",
                        column: x => x.BreachId,
                        principalTable: "Breach",
                        principalColumn: "id_breach",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Breach_Detail_Breach_List_BreachListId",
                        column: x => x.BreachListId,
                        principalTable: "Breach_List",
                        principalColumn: "id_breach_list",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breach_Warranty",
                columns: table => new
                {
                    id_breach_warranty = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    references = table.Column<string>(maxLength: 100, nullable: false),
                    observations = table.Column<string>(maxLength: 256, nullable: false),
                    BreachId = table.Column<int>(nullable: false),
                    WarrantyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breach_Warranty", x => x.id_breach_warranty);
                    table.ForeignKey(
                        name: "FK_Breach_Warranty_Breach_BreachId",
                        column: x => x.BreachId,
                        principalTable: "Breach",
                        principalColumn: "id_breach",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Breach_Warranty_Warranty_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranty",
                        principalColumn: "id_breach_warranty",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Ticket_UserId",
                table: "Assignment_Ticket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_TaxUserId",
                table: "Breach",
                column: "TaxUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_UserId",
                table: "Breach",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_Detail_BreachId",
                table: "Breach_Detail",
                column: "BreachId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_Detail_BreachListId",
                table: "Breach_Detail",
                column: "BreachListId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_List_BreachArticleId",
                table: "Breach_List",
                column: "BreachArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_Warranty_BreachId",
                table: "Breach_Warranty",
                column: "BreachId");

            migrationBuilder.CreateIndex(
                name: "IX_Breach_Warranty_WarrantyId",
                table: "Breach_Warranty",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Sale_DivisionId",
                table: "Order_Sale",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Sale_Detail_OrderSaleId",
                table: "Order_Sale_Detail",
                column: "OrderSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_Address_TaxUserId",
                table: "Tax_Address",
                column: "TaxUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignment_Ticket");

            migrationBuilder.DropTable(
                name: "Breach_Detail");

            migrationBuilder.DropTable(
                name: "Breach_Warranty");

            migrationBuilder.DropTable(
                name: "Order_Sale_Detail");

            migrationBuilder.DropTable(
                name: "Tax_Address");

            migrationBuilder.DropTable(
                name: "Breach_List");

            migrationBuilder.DropTable(
                name: "Breach");

            migrationBuilder.DropTable(
                name: "Warranty");

            migrationBuilder.DropTable(
                name: "Order_Sale");

            migrationBuilder.DropTable(
                name: "Breach_Article");

            migrationBuilder.DropTable(
                name: "Tax_User");

            migrationBuilder.DropColumn(
                name: "id_solution",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Division");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 28, 16, 41, 12, 642, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 1, 16, 4, 43, 63, DateTimeKind.Local));
        }
    }
}
