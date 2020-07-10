using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addValueToVALVULACONTROL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 10, 15, 47, 59, 530, DateTimeKind.Local).AddTicks(3290),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 2, 17, 56, 47, 879, DateTimeKind.Local).AddTicks(7899));

            migrationBuilder.InsertData(
                table: "valvula_control",
                columns: new[] { "id_valvula_control", "actual_state", "description", "diameter", "hydraulic_circuit", "is_active", "last_service_date", "latitude", "longitude", "physical_state", "reference", "type" },
                values: new object[] { 1, "", "Sin Valvula", "", "", true, new DateTime(2020, 7, 10, 15, 47, 59, 654, DateTimeKind.Local).AddTicks(5568), "19.0954579", "-98.2792209", "", "Sin referencia", "OT011" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 2, 17, 56, 47, 879, DateTimeKind.Local).AddTicks(7899),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 10, 15, 47, 59, 530, DateTimeKind.Local).AddTicks(3290));
        }
    }
}
