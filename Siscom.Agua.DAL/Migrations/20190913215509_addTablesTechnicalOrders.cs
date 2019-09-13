using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablesTechnicalOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 13, 16, 55, 8, 265, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 12, 17, 31, 31, 213, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Order_work",
                columns: table => new
                {
                    id_order_work = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 30, nullable: false),
                    date_order = table.Column<DateTime>(nullable: false),
                    applicant = table.Column<string>(maxLength: 150, nullable: false),
                    type = table.Column<string>(maxLength: 6, nullable: false),
                    status = table.Column<string>(maxLength: 6, nullable: false),
                    observation = table.Column<string>(maxLength: 800, nullable: true),
                    date_stimated = table.Column<DateTime>(nullable: false),
                    date_realization = table.Column<DateTime>(nullable: false),
                    activities = table.Column<string>(maxLength: 250, nullable: false),
                    AgreementId = table.Column<int>(nullable: false),
                    TaxUserId = table.Column<int>(nullable: false),
                    TechnicalStaffId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_work", x => x.id_order_work);
                });

            migrationBuilder.CreateTable(
                name: "Order_Work_Parameters",
                columns: table => new
                {
                    id_order_work_parameters = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    is_agreement = table.Column<bool>(nullable: false),
                    type_order = table.Column<string>(nullable: false),
                    status_order = table.Column<string>(nullable: false),
                    TypeIntakeId = table.Column<string>(nullable: true),
                    code_concept = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Work_Parameters", x => x.id_order_work_parameters);
                });

            migrationBuilder.CreateTable(
                name: "Technical_Role",
                columns: table => new
                {
                    id_technical_role = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technical_Role", x => x.id_technical_role);
                });

            migrationBuilder.CreateTable(
                name: "Technical_Team",
                columns: table => new
                {
                    id_technical_team = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(maxLength: 150, nullable: false),
                    name = table.Column<string>(maxLength: 60, nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technical_Team", x => x.id_technical_team);
                });

            migrationBuilder.CreateTable(
                name: "Order_work_Status",
                columns: table => new
                {
                    id_order_work_status = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_status = table.Column<string>(maxLength: 10, nullable: false),
                    order_work_status_date = table.Column<DateTime>(nullable: false),
                    user = table.Column<string>(maxLength: 80, nullable: false),
                    OrderWorkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_work_Status", x => x.id_order_work_status);
                    table.ForeignKey(
                        name: "FK_Order_work_Status_Order_work_OrderWorkId",
                        column: x => x.OrderWorkId,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Technical_Staff",
                columns: table => new
                {
                    id_technical_staff = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 150, nullable: false),
                    phone = table.Column<string>(nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    TechnicalRoleId = table.Column<int>(nullable: false),
                    TechnicalTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technical_Staff", x => x.id_technical_staff);
                    table.ForeignKey(
                        name: "FK_Technical_Staff_Technical_Role_TechnicalRoleId",
                        column: x => x.TechnicalRoleId,
                        principalTable: "Technical_Role",
                        principalColumn: "id_technical_role",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Technical_Staff_Technical_Team_TechnicalTeamId",
                        column: x => x.TechnicalTeamId,
                        principalTable: "Technical_Team",
                        principalColumn: "id_technical_team",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_work_Status_OrderWorkId",
                table: "Order_work_Status",
                column: "OrderWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_Technical_Staff_TechnicalRoleId",
                table: "Technical_Staff",
                column: "TechnicalRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Technical_Staff_TechnicalTeamId",
                table: "Technical_Staff",
                column: "TechnicalTeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order_Work_Parameters");

            migrationBuilder.DropTable(
                name: "Order_work_Status");

            migrationBuilder.DropTable(
                name: "Technical_Staff");

            migrationBuilder.DropTable(
                name: "Order_work");

            migrationBuilder.DropTable(
                name: "Technical_Role");

            migrationBuilder.DropTable(
                name: "Technical_Team");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 12, 17, 31, 31, 213, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 13, 16, 55, 8, 265, DateTimeKind.Local));
        }
    }
}
