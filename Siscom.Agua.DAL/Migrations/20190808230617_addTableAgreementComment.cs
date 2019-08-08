using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableAgreementComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_provider",
                table: "Tax_User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 8, 8, 18, 6, 16, 980, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 27, 13, 44, 27, 502, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "agreement_comment",
                columns: table => new
                {
                    id_agreement_comment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date_in = table.Column<DateTime>(nullable: false),
                    observation = table.Column<string>(maxLength: 800, nullable: true),
                    is_visible = table.Column<bool>(nullable: false),
                    user_id = table.Column<string>(maxLength: 50, nullable: true),
                    user_name = table.Column<string>(maxLength: 50, nullable: true),
                    date_out = table.Column<DateTime>(nullable: false),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agreement_comment", x => x.id_agreement_comment);
                    table.ForeignKey(
                        name: "FK_agreement_comment_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agreement_comment_AgreementId",
                table: "agreement_comment",
                column: "AgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agreement_comment");

            migrationBuilder.DropColumn(
                name: "is_provider",
                table: "Tax_User");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 27, 13, 44, 27, 502, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 8, 8, 18, 6, 16, 980, DateTimeKind.Local));
        }
    }
}
