using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.CreateTable(
                name: "Debt_Period",
                columns: table => new
                {
                    id_debt_period = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    period = table.Column<short>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    run_date = table.Column<DateTime>(type: "date", nullable: false),
                    run_hour = table.Column<TimeSpan>(type: "time", nullable: false),
                    TypePeriodId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Period", x => x.id_debt_period);
                    table.ForeignKey(
                        name: "FK_Debt_Period_Type_Period_TypePeriodId",
                        column: x => x.TypePeriodId,
                        principalTable: "Type_Period",
                        principalColumn: "id_type_period",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Period_TypePeriodId",
                table: "Debt_Period",
                column: "TypePeriodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt_Period");

        }
    }
}
