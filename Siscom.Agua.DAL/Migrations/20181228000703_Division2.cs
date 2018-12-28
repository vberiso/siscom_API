using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Division2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_variable",
                table: "Tariff_Product",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "percentage",
                table: "Tariff_Product",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "times_factor",
                table: "Tariff_Product",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Product",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 18, 7, 3, 2, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 11, 57, 19, 818, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Debt_Calculation",
                columns: table => new
                {
                    id_debt_calculation = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<decimal>(nullable: false),
                    percentage = table.Column<short>(nullable: false),
                    factor = table.Column<short>(nullable: false),
                    times_factor = table.Column<short>(nullable: false),
                    is_variable = table.Column<bool>(nullable: false),
                    DebtId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Calculation", x => x.id_debt_calculation);
                    table.ForeignKey(
                        name: "FK_Debt_Calculation_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    id_division = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.id_division);
                });

            migrationBuilder.CreateTable(
                name: "Push_Notification",
                columns: table => new
                {
                    id_notification = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(nullable: false),
                    agreement_id = table.Column<int>(nullable: false),
                    debt_id = table.Column<int>(nullable: false),
                    folio = table.Column<string>(maxLength: 40, nullable: true),
                    porcentage = table.Column<byte>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    reason = table.Column<string>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Push_Notification", x => x.id_notification);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_DivisionId",
                table: "Product",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Calculation_DebtId",
                table: "Debt_Calculation",
                column: "DebtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Division_DivisionId",
                table: "Product",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "id_division",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Division_DivisionId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "Debt_Calculation");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "Push_Notification");

            migrationBuilder.DropIndex(
                name: "IX_Product_DivisionId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "is_variable",
                table: "Tariff_Product");

            migrationBuilder.DropColumn(
                name: "percentage",
                table: "Tariff_Product");

            migrationBuilder.DropColumn(
                name: "times_factor",
                table: "Tariff_Product");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Product");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 11, 57, 19, 818, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 18, 7, 3, 2, DateTimeKind.Local));
        }
    }
}
