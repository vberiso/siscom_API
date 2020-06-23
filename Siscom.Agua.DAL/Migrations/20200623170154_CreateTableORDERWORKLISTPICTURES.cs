using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class CreateTableORDERWORKLISTPICTURES : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 23, 12, 1, 53, 66, DateTimeKind.Local).AddTicks(3308),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 18, 11, 22, 2, 859, DateTimeKind.Local).AddTicks(8379));

            migrationBuilder.CreateTable(
                name: "OrderWorkListPictures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(nullable: true),
                    capture_date = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    size = table.Column<long>(nullable: false),
                    weight = table.Column<string>(maxLength: 10, nullable: true),
                    user = table.Column<string>(nullable: true),
                    user_name = table.Column<string>(nullable: true),
                    file_picture = table.Column<byte[]>(nullable: true),
                    OrderWorkListId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderWorkListPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderWorkListPictures_order_work_list_OrderWorkListId",
                        column: x => x.OrderWorkListId,
                        principalTable: "order_work_list",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderWorkListPictures_OrderWorkListId",
                table: "OrderWorkListPictures",
                column: "OrderWorkListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderWorkListPictures");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 18, 11, 22, 2, 859, DateTimeKind.Local).AddTicks(8379),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 23, 12, 1, 53, 66, DateTimeKind.Local).AddTicks(3308));
        }
    }
}
