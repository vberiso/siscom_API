using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableLocationOrderWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 15, 16, 50, 7, 674, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 14, 22, 12, 40, 281, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "LocationOfAttentionOrderWorks",
                columns: table => new
                {
                    id_Location = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    latitude = table.Column<string>(maxLength: 20, nullable: true),
                    longitude = table.Column<string>(maxLength: 20, nullable: true),
                    type = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationOfAttentionOrderWorks", x => x.id_Location);
                });

            migrationBuilder.CreateTable(
                name: "LocationOrderWorks",
                columns: table => new
                {
                    OrderWorkId = table.Column<int>(nullable: false),
                    LocationOfAttentionOrderWorkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationOrderWorks", x => new { x.OrderWorkId, x.LocationOfAttentionOrderWorkId });
                    table.ForeignKey(
                        name: "FK_LocationOrderWorks_LocationOfAttentionOrderWorks_LocationOfAttentionOrderWorkId",
                        column: x => x.LocationOfAttentionOrderWorkId,
                        principalTable: "LocationOfAttentionOrderWorks",
                        principalColumn: "id_Location",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationOrderWorks_Order_work_OrderWorkId",
                        column: x => x.OrderWorkId,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationOrderWorks_LocationOfAttentionOrderWorkId",
                table: "LocationOrderWorks",
                column: "LocationOfAttentionOrderWorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationOrderWorks");

            migrationBuilder.DropTable(
                name: "LocationOfAttentionOrderWorks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 14, 22, 12, 40, 281, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 4, 15, 16, 50, 7, 674, DateTimeKind.Local));
        }
    }
}
