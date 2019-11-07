using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AddTablePhotosOrderWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 7, 11, 22, 7, 110, DateTimeKind.Local).AddTicks(3073),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 5, 16, 59, 47, 872, DateTimeKind.Local).AddTicks(127));

            migrationBuilder.CreateTable(
                name: "PhotosOrderWork",
                columns: table => new
                {
                    id_photos_orderWork = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderWorkId = table.Column<int>(nullable: false),
                    path_file = table.Column<string>(nullable: true),
                    date_photo = table.Column<string>(nullable: true),
                    user = table.Column<string>(nullable: true),
                    user_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosOrderWork", x => x.id_photos_orderWork);
                    table.ForeignKey(
                        name: "FK_PhotosOrderWork_Order_work_OrderWorkId",
                        column: x => x.OrderWorkId,
                        principalTable: "Order_work",
                        principalColumn: "id_order_work",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotosOrderWork_OrderWorkId",
                table: "PhotosOrderWork",
                column: "OrderWorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotosOrderWork");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 5, 16, 59, 47, 872, DateTimeKind.Local).AddTicks(127),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 7, 11, 22, 7, 110, DateTimeKind.Local).AddTicks(3073));
        }
    }
}
