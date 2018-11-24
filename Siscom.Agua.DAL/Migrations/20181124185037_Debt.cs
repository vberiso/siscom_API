using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Debt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.CreateTable(
                name: "Debt",
                columns: table => new
                {
                    id_debt = table.Column<int>(nullable: false),
                    debit_date = table.Column<DateTime>(nullable: false),
                    from_date = table.Column<DateTime>(type: "date", nullable: false),
                    until_date = table.Column<DateTime>(type: "date", nullable: false),
                    derivatives = table.Column<int>(nullable: false),
                    type_intake = table.Column<string>(maxLength: 50, nullable: false),
                    type_service = table.Column<string>(maxLength: 50, nullable: false),
                    consumption = table.Column<string>(maxLength: 10, nullable: false),
                    discount = table.Column<string>(maxLength: 50, nullable: true),
                    amount = table.Column<double>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    year = table.Column<short>(nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    DebtPeriodId = table.Column<int>(nullable: true),
                    AgreementId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt", x => x.id_debt);
                    table.ForeignKey(
                        name: "FK_Debt_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debt_Debt_Period_DebtPeriodId",
                        column: x => x.DebtPeriodId,
                        principalTable: "Debt_Period",
                        principalColumn: "id_debt_period",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Group_Status",
                columns: table => new
                {
                    id_group_type = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Status", x => x.id_group_type);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    id_type = table.Column<string>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    GroupStatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.id_type);
                    table.ForeignKey(
                        name: "FK_Status_Group_Status_GroupStatusId",
                        column: x => x.GroupStatusId,
                        principalTable: "Group_Status",
                        principalColumn: "id_group_type",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debt_AgreementId",
                table: "Debt",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_DebtPeriodId",
                table: "Debt",
                column: "DebtPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_GroupStatusId",
                table: "Status",
                column: "GroupStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debt");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Group_Status");
           
        }
    }
}
