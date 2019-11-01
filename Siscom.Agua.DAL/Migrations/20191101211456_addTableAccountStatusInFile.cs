using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableAccountStatusInFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 1, 15, 14, 55, 94, DateTimeKind.Local).AddTicks(2950),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 1, 11, 36, 29, 72, DateTimeKind.Local).AddTicks(3743));

            migrationBuilder.CreateTable(
                name: "account_status_In_File",
                columns: table => new
                {
                    account_status_In_File_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>(nullable: true),
                    user_name = table.Column<string>(nullable: true),
                    generation_date = table.Column<DateTime>(nullable: false),
                    file_name = table.Column<string>(maxLength: 200, nullable: true),
                    folio = table.Column<string>(nullable: true),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_status_In_File", x => x.account_status_In_File_id);
                    table.ForeignKey(
                        name: "FK_account_status_In_File_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_status_In_File_AgreementId",
                table: "account_status_In_File",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 1, 11, 36, 29, 72, DateTimeKind.Local).AddTicks(3743),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 1, 15, 14, 55, 94, DateTimeKind.Local).AddTicks(2950));
        }
    }
}
