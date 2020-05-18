using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablesMaterials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 18, 15, 49, 38, 611, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 12, 16, 19, 20, 132, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "MaterialLists",
                columns: table => new
                {
                    id_material = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 60, nullable: true),
                    code = table.Column<string>(maxLength: 25, nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialLists", x => x.id_material);
                });

            migrationBuilder.CreateTable(
                name: "material_movements",
                columns: table => new
                {
                    orderWork_id = table.Column<int>(nullable: false),
                    materialList_id = table.Column<int>(nullable: false),
                    movement_date = table.Column<DateTime>(type: "date", nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: true),
                    quantity = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_material_movements", x => new { x.materialList_id, x.orderWork_id });
                    table.ForeignKey(
                        name: "FK_material_movements_MaterialLists_materialList_id",
                        column: x => x.materialList_id,
                        principalTable: "MaterialLists",
                        principalColumn: "id_material",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_material_movements_Order_work_orderWork_id",
                        column: x => x.orderWork_id,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitMeasurements",
                columns: table => new
                {
                    id_unit_measurement = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    acronym = table.Column<string>(maxLength: 10, nullable: true),
                    description = table.Column<string>(maxLength: 60, nullable: true),
                    IActive = table.Column<bool>(nullable: false),
                    MaterialListId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitMeasurements", x => x.id_unit_measurement);
                    table.ForeignKey(
                        name: "FK_UnitMeasurements_MaterialLists_MaterialListId",
                        column: x => x.MaterialListId,
                        principalTable: "MaterialLists",
                        principalColumn: "id_material",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_material_movements_orderWork_id",
                table: "material_movements",
                column: "orderWork_id");

            migrationBuilder.CreateIndex(
                name: "IX_UnitMeasurements_MaterialListId",
                table: "UnitMeasurements",
                column: "MaterialListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "material_movements");

            migrationBuilder.DropTable(
                name: "UnitMeasurements");

            migrationBuilder.DropTable(
                name: "MaterialLists");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 5, 12, 16, 19, 20, 132, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 5, 18, 15, 49, 38, 611, DateTimeKind.Local));
        }
    }
}
