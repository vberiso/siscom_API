using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableOrderWorkDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 27, 20, 3, 44, 716, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 24, 10, 8, 51, 59, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "order_work_detail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 40, nullable: true),
                    type = table.Column<string>(maxLength: 5, nullable: true),
                    value = table.Column<string>(maxLength: 150, nullable: true),
                    OrderWorkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_work_detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_work_detail_Order_work_OrderWorkId",
                        column: x => x.OrderWorkId,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_work_detail_OrderWorkId",
                table: "order_work_detail",
                column: "OrderWorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_work_detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 24, 10, 8, 51, 59, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 27, 20, 3, 44, 716, DateTimeKind.Local));
        }
    }
}
