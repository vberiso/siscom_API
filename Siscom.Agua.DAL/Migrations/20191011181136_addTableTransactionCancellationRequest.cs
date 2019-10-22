using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableTransactionCancellationRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 11, 13, 11, 35, 793, DateTimeKind.Local).AddTicks(4305),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 9, 16, 57, 31, 434, DateTimeKind.Local).AddTicks(9282));

            migrationBuilder.CreateTable(
                name: "Transaction_Cancelation_Request",
                columns: table => new
                {
                    id_transaction_cancelation_request = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(nullable: true),
                    DateRequest = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    DateAuthorization = table.Column<DateTime>(nullable: false),
                    ManagerObservation = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Key_Firebase = table.Column<string>(maxLength: 50, nullable: true),
                    TransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction_Cancelation_Request", x => x.id_transaction_cancelation_request);
                    table.ForeignKey(
                        name: "FK_Transaction_Cancelation_Request_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "id_transaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Cancelation_Request_TransactionId",
                table: "Transaction_Cancelation_Request",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction_Cancelation_Request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 9, 16, 57, 31, 434, DateTimeKind.Local).AddTicks(9282),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 11, 13, 11, 35, 793, DateTimeKind.Local).AddTicks(4305));
        }
    }
}
