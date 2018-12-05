using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TypeClasif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debt_Detail_Service_ServiceId",
                table: "Debt_Detail");

            migrationBuilder.DropForeignKey(
                name: "FK_Tariff_Type_Intake_TypeIntakeId",
                table: "Tariff");

            migrationBuilder.DropForeignKey(
                name: "FK_Tariff_Type_Service_TypeServiceId",
                table: "Tariff");

            migrationBuilder.DropForeignKey(
                name: "FK_Tariff_Type_Use_TypeUseId",
                table: "Tariff");

            migrationBuilder.DropIndex(
                name: "IX_Tariff_TypeIntakeId",
                table: "Tariff");

            migrationBuilder.DropIndex(
                name: "IX_Tariff_TypeServiceId",
                table: "Tariff");

            migrationBuilder.DropIndex(
                name: "IX_Tariff_TypeUseId",
                table: "Tariff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Debt_Detail",
                table: "Debt_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Debt_Detail_ServiceId",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "TypeIntakeId",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "TypeServiceId",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "TypeUseId",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "have_tax",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "in_agreement",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Debt_Detail");

            migrationBuilder.RenameColumn(
                name: "is_service",
                table: "Service",
                newName: "is_commercial");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 5, 16, 57, 9, 279, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 5, 13, 56, 54, 138, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "id_debt_detail",
                table: "Debt_Detail",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "code_concept",
                table: "Debt_Detail",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name_concept",
                table: "Debt_Detail",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Debt_Detail",
                table: "Debt_Detail",
                column: "id_debt_detail");

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    id_service = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    order = table.Column<short>(nullable: false),
                    is_service = table.Column<bool>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    in_agreement = table.Column<bool>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.id_service);
                });

            migrationBuilder.CreateTable(
                name: "Tariff_Product",
                columns: table => new
                {
                    id_tariff = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    concept = table.Column<string>(maxLength: 80, nullable: false),
                    account_number = table.Column<string>(maxLength: 20, nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    percentage = table.Column<short>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    from_date = table.Column<DateTime>(nullable: false),
                    until_date = table.Column<DateTime>(nullable: false),
                    is_active = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariff_Product", x => x.id_tariff);
                    table.ForeignKey(
                        name: "FK_Tariff_Product_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_DebtId",
                table: "Debt_Detail",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_Product_ProductId",
                table: "Tariff_Product",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tariff_Product");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Debt_Detail",
                table: "Debt_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Debt_Detail_DebtId",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "id_debt_detail",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "code_concept",
                table: "Debt_Detail");

            migrationBuilder.DropColumn(
                name: "name_concept",
                table: "Debt_Detail");

            migrationBuilder.RenameColumn(
                name: "is_commercial",
                table: "Service",
                newName: "is_service");

            migrationBuilder.AddColumn<int>(
                name: "TypeIntakeId",
                table: "Tariff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeServiceId",
                table: "Tariff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeUseId",
                table: "Tariff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "have_tax",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "in_agreement",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 5, 13, 56, 54, 138, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 5, 16, 57, 9, 279, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Debt_Detail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Debt_Detail",
                table: "Debt_Detail",
                columns: new[] { "DebtId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeIntakeId",
                table: "Tariff",
                column: "TypeIntakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeServiceId",
                table: "Tariff",
                column: "TypeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeUseId",
                table: "Tariff",
                column: "TypeUseId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_ServiceId",
                table: "Debt_Detail",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Debt_Detail_Service_ServiceId",
                table: "Debt_Detail",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "id_service",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tariff_Type_Intake_TypeIntakeId",
                table: "Tariff",
                column: "TypeIntakeId",
                principalTable: "Type_Intake",
                principalColumn: "id_type_intake",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tariff_Type_Service_TypeServiceId",
                table: "Tariff",
                column: "TypeServiceId",
                principalTable: "Type_Service",
                principalColumn: "id_type_service",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tariff_Type_Use_TypeUseId",
                table: "Tariff",
                column: "TypeUseId",
                principalTable: "Type_Use",
                principalColumn: "id_type_use",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
