using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TransactionFolio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folio_initial",
                table: "Folio");

            migrationBuilder.AlterColumn<string>(
                name: "folio",
                table: "Transaction",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 19, 12, 37, 37, 564, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "is_active",
                table: "Folio",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "range",
                table: "Folio",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Transaction_Folio",
                columns: table => new
                {
                    folio = table.Column<string>(nullable: false),
                    DatePrint = table.Column<DateTime>(nullable: false),
                    TransactionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction_Folio", x => x.folio);
                    table.ForeignKey(
                        name: "FK_Transaction_Folio_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "id_transaction",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Folio_TransactionId",
                table: "Transaction_Folio",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction_Folio");

            migrationBuilder.DropColumn(
                name: "current_date",
                table: "Folio");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Folio");

            migrationBuilder.DropColumn(
                name: "range",
                table: "Folio");

            migrationBuilder.AlterColumn<string>(
                name: "folio",
                table: "Transaction",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folio_initial",
                table: "Folio",
                column: "initial",
                unique: true);
        }
    }
}
