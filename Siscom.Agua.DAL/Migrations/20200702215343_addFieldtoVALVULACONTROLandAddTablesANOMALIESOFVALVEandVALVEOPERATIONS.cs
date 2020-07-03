using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldtoVALVULACONTROLandAddTablesANOMALIESOFVALVEandVALVEOPERATIONS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "actual_state",
                table: "valvula_control",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "diameter",
                table: "valvula_control",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hydraulic_circuit",
                table: "valvula_control",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_service_date",
                table: "valvula_control",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "physical_state",
                table: "valvula_control",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 2, 16, 53, 42, 131, DateTimeKind.Local).AddTicks(7196),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 30, 16, 33, 46, 906, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "anomalies_of_valve",
                columns: table => new
                {
                    id_anomalies_of_valve = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(maxLength: 5, nullable: true),
                    anomalies_date = table.Column<DateTime>(nullable: false),
                    observations = table.Column<string>(maxLength: 300, nullable: true),
                    atention_date = table.Column<DateTime>(nullable: false),
                    work_description = table.Column<string>(maxLength: 300, nullable: true),
                    ValvulaControlId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anomalies_of_valve", x => x.id_anomalies_of_valve);
                    table.ForeignKey(
                        name: "FK_anomalies_of_valve_valvula_control_ValvulaControlId",
                        column: x => x.ValvulaControlId,
                        principalTable: "valvula_control",
                        principalColumn: "id_valvula_control",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValveOperations",
                columns: table => new
                {
                    id_valve_operations = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OperationStart = table.Column<DateTime>(nullable: false),
                    OperationEnd = table.Column<DateTime>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    ValvulaControlId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValveOperations", x => x.id_valve_operations);
                    table.ForeignKey(
                        name: "FK_ValveOperations_valvula_control_ValvulaControlId",
                        column: x => x.ValvulaControlId,
                        principalTable: "valvula_control",
                        principalColumn: "id_valvula_control",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_anomalies_of_valve_ValvulaControlId",
                table: "anomalies_of_valve",
                column: "ValvulaControlId");

            migrationBuilder.CreateIndex(
                name: "IX_ValveOperations_ValvulaControlId",
                table: "ValveOperations",
                column: "ValvulaControlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "anomalies_of_valve");

            migrationBuilder.DropTable(
                name: "ValveOperations");

            migrationBuilder.DropColumn(
                name: "actual_state",
                table: "valvula_control");

            migrationBuilder.DropColumn(
                name: "diameter",
                table: "valvula_control");

            migrationBuilder.DropColumn(
                name: "hydraulic_circuit",
                table: "valvula_control");

            migrationBuilder.DropColumn(
                name: "last_service_date",
                table: "valvula_control");

            migrationBuilder.DropColumn(
                name: "physical_state",
                table: "valvula_control");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 30, 16, 33, 46, 906, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 2, 16, 53, 42, 131, DateTimeKind.Local).AddTicks(7196));
        }
    }
}
