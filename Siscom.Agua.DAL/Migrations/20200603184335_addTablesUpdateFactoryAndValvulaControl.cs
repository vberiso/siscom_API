using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablesUpdateFactoryAndValvulaControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 3, 13, 43, 35, 8, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 26, 17, 7, 16, 783, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "previous_debtId",
                table: "Debt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "debt_update_factory",
                columns: table => new
                {
                    id_debt_update_factor = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    change_date = table.Column<DateTime>(nullable: false),
                    code_concept = table.Column<string>(maxLength: 5, nullable: true),
                    name_concept = table.Column<string>(maxLength: 5, nullable: true),
                    original_amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    change_amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DebtId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debt_update_factory", x => x.id_debt_update_factor);
                    table.ForeignKey(
                        name: "FK_debt_update_factory_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "valvula_control",
                columns: table => new
                {
                    id_valvula_control = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 150, nullable: true),
                    reference = table.Column<string>(maxLength: 300, nullable: true),
                    latitude = table.Column<string>(maxLength: 20, nullable: true),
                    longitude = table.Column<string>(maxLength: 20, nullable: true),
                    type = table.Column<string>(maxLength: 5, nullable: true),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_valvula_control", x => x.id_valvula_control);
                });

            migrationBuilder.CreateIndex(
                name: "IX_debt_update_factory_DebtId",
                table: "debt_update_factory",
                column: "DebtId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "debt_update_factory");

            migrationBuilder.DropTable(
                name: "valvula_control");

            migrationBuilder.DropColumn(
                name: "previous_debtId",
                table: "Debt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 26, 17, 7, 16, 783, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 3, 13, 43, 35, 8, DateTimeKind.Local));
        }
    }
}
