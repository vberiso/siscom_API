using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class changeNameTableINCIDENCIAoFVALVE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValveOperations_valvula_control_ValvulaControlId",
                table: "ValveOperations");

            migrationBuilder.DropTable(
                name: "anomalies_of_valve");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValveOperations",
                table: "ValveOperations");

            migrationBuilder.RenameTable(
                name: "ValveOperations",
                newName: "valve_operation");

            migrationBuilder.RenameIndex(
                name: "IX_ValveOperations_ValvulaControlId",
                table: "valve_operation",
                newName: "IX_valve_operation_ValvulaControlId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 2, 17, 56, 47, 879, DateTimeKind.Local).AddTicks(7899),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 2, 16, 53, 42, 131, DateTimeKind.Local).AddTicks(7196));

            migrationBuilder.AddPrimaryKey(
                name: "PK_valve_operation",
                table: "valve_operation",
                column: "id_valve_operations");

            migrationBuilder.CreateTable(
                name: "valve_incident",
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
                    table.PrimaryKey("PK_valve_incident", x => x.id_anomalies_of_valve);
                    table.ForeignKey(
                        name: "FK_valve_incident_valvula_control_ValvulaControlId",
                        column: x => x.ValvulaControlId,
                        principalTable: "valvula_control",
                        principalColumn: "id_valvula_control",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_valve_incident_ValvulaControlId",
                table: "valve_incident",
                column: "ValvulaControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_valve_operation_valvula_control_ValvulaControlId",
                table: "valve_operation",
                column: "ValvulaControlId",
                principalTable: "valvula_control",
                principalColumn: "id_valvula_control",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_valve_operation_valvula_control_ValvulaControlId",
                table: "valve_operation");

            migrationBuilder.DropTable(
                name: "valve_incident");

            migrationBuilder.DropPrimaryKey(
                name: "PK_valve_operation",
                table: "valve_operation");

            migrationBuilder.RenameTable(
                name: "valve_operation",
                newName: "ValveOperations");

            migrationBuilder.RenameIndex(
                name: "IX_valve_operation_ValvulaControlId",
                table: "ValveOperations",
                newName: "IX_ValveOperations_ValvulaControlId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 2, 16, 53, 42, 131, DateTimeKind.Local).AddTicks(7196),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 2, 17, 56, 47, 879, DateTimeKind.Local).AddTicks(7899));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValveOperations",
                table: "ValveOperations",
                column: "id_valve_operations");

            migrationBuilder.CreateTable(
                name: "anomalies_of_valve",
                columns: table => new
                {
                    id_anomalies_of_valve = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    anomalies_date = table.Column<DateTime>(nullable: false),
                    atention_date = table.Column<DateTime>(nullable: false),
                    observations = table.Column<string>(maxLength: 300, nullable: true),
                    type = table.Column<string>(maxLength: 5, nullable: true),
                    ValvulaControlId = table.Column<int>(nullable: false),
                    work_description = table.Column<string>(maxLength: 300, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_anomalies_of_valve_ValvulaControlId",
                table: "anomalies_of_valve",
                column: "ValvulaControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_ValveOperations_valvula_control_ValvulaControlId",
                table: "ValveOperations",
                column: "ValvulaControlId",
                principalTable: "valvula_control",
                principalColumn: "id_valvula_control",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
