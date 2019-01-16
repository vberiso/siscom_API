using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class CancelAuthorization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_cancel_authorization",
                table: "Transaction",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 15, 18, 0, 2, 246, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 15, 15, 18, 14, 667, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Cancel_Authorization",
                columns: table => new
                {
                    id_cancel_authorization = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_terminal_user = table.Column<int>(nullable: false),
                    id_branch_office = table.Column<int>(nullable: false),
                    id_transaction = table.Column<int>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    id_user = table.Column<string>(nullable: true),
                    date_authorization = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancel_Authorization", x => x.id_cancel_authorization);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cancel_Authorization");

            migrationBuilder.DropColumn(
                name: "id_cancel_authorization",
                table: "Transaction");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 15, 15, 18, 14, 667, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 15, 18, 0, 2, 246, DateTimeKind.Local));
        }
    }
}
