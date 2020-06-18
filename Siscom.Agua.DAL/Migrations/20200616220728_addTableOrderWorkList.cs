using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableOrderWorkList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 16, 17, 7, 26, 752, DateTimeKind.Local).AddTicks(3624),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 11, 15, 36, 17, 600, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "order_work_list",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    agreement_id = table.Column<int>(nullable: false),
                    status = table.Column<string>(maxLength: 6, nullable: true),
                    folio_order = table.Column<string>(maxLength: 30, nullable: true),
                    type_order = table.Column<string>(maxLength: 5, nullable: true),
                    OrderWorkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_work_list", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_work_list_Order_work_OrderWorkId",
                        column: x => x.OrderWorkId,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_work_list_OrderWorkId",
                table: "order_work_list",
                column: "OrderWorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_work_list");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 11, 15, 36, 17, 600, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 16, 17, 7, 26, 752, DateTimeKind.Local).AddTicks(3624));
        }
    }
}
