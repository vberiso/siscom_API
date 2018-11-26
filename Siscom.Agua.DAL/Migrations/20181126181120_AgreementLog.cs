using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 26, 12, 11, 19, 813, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 26, 11, 32, 5, 401, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Agreement_Log",
                columns: table => new
                {
                    id_agreement_log = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    agreement_log_date = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    observation = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_Log", x => x.id_agreement_log);
                    table.ForeignKey(
                        name: "FK_Agreement_Log_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Log_UserId",
                table: "Agreement_Log",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agreement_Log");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 26, 11, 32, 5, 401, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 26, 12, 11, 19, 813, DateTimeKind.Local));
        }
    }
}
