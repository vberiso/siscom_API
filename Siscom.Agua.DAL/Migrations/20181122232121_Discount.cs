using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Discount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "mounth",
                table: "Type_Period",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 22, 17, 21, 20, 975, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 21, 12, 31, 10, 6, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    id_discount = table.Column<int>(nullable: false),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    percentpercentage = table.Column<short>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    ValidityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.id_discount);
                    table.ForeignKey(
                        name: "FK_Discount_Type_Period_ValidityId",
                        column: x => x.ValidityId,
                        principalTable: "Type_Period",
                        principalColumn: "id_type_period",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agreement_Discount",
                columns: table => new
                {
                    id_discount = table.Column<int>(nullable: false),
                    id_agreement = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_Discount", x => new { x.id_discount, x.id_agreement });
                    table.ForeignKey(
                        name: "FK_Agreement_Discount_Agreement_id_agreement",
                        column: x => x.id_agreement,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Discount_Discount_id_discount",
                        column: x => x.id_discount,
                        principalTable: "Discount",
                        principalColumn: "id_discount",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Discount_id_agreement",
                table: "Agreement_Discount",
                column: "id_agreement");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_ValidityId",
                table: "Discount",
                column: "ValidityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agreement_Discount");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropColumn(
                name: "mounth",
                table: "Type_Period");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 12, 31, 10, 6, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 22, 17, 21, 20, 975, DateTimeKind.Local));
        }
    }
}
