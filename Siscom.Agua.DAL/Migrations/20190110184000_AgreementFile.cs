using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 10, 12, 39, 59, 939, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 9, 17, 29, 59, 592, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "manifest",
                table: "Agreement_Detail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Agreement_File",
                columns: table => new
                {
                    id_agreement_file = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    extension = table.Column<string>(maxLength: 4, nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_File", x => x.id_agreement_file);
                    table.ForeignKey(
                        name: "FK_Agreement_File_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_File_AgreementId",
                table: "Agreement_File",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agreement_File");

            migrationBuilder.DropColumn(
                name: "manifest",
                table: "Agreement_Detail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 9, 17, 29, 59, 592, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 10, 12, 39, 59, 939, DateTimeKind.Local));
        }
    }
}
