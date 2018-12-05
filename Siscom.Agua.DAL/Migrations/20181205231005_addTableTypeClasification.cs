using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableTypeClasification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 5, 17, 10, 5, 14, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 5, 16, 57, 9, 279, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "TypeClassificationId",
                table: "Agreement",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Type_Classification",
                columns: table => new
                {
                    id_type_classification = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    intake_acronym = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Classification", x => x.id_type_classification);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeClassificationId",
                table: "Agreement",
                column: "TypeClassificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_Type_Classification_TypeClassificationId",
                table: "Agreement",
                column: "TypeClassificationId",
                principalTable: "Type_Classification",
                principalColumn: "id_type_classification",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_Type_Classification_TypeClassificationId",
                table: "Agreement");

            migrationBuilder.DropTable(
                name: "Type_Classification");

            migrationBuilder.DropIndex(
                name: "IX_Agreement_TypeClassificationId",
                table: "Agreement");

            migrationBuilder.DropColumn(
                name: "TypeClassificationId",
                table: "Agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 5, 16, 57, 9, 279, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 5, 17, 10, 5, 14, DateTimeKind.Local));
        }
    }
}
