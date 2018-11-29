using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DiscountIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 18, 29, 56, 44, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 18, 27, 8, 878, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    id_discount = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    percentage = table.Column<short>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    TypePeriodId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.id_discount);
                    table.ForeignKey(
                        name: "FK_Discount_Type_Period_TypePeriodId",
                        column: x => x.TypePeriodId,
                        principalTable: "Type_Period",
                        principalColumn: "id_type_period",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discount_TypePeriodId",
                table: "Discount",
                column: "TypePeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Discount_Discount_id_discount",
                table: "Agreement_Discount",
                column: "id_discount",
                principalTable: "Discount",
                principalColumn: "id_discount",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Discount_Discount_id_discount",
                table: "Agreement_Discount");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 28, 18, 27, 8, 878, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 28, 18, 29, 56, 44, DateTimeKind.Local));
        }
    }
}
