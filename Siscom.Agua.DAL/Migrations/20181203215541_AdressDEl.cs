using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AdressDEl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 3, 15, 55, 40, 240, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 3, 14, 7, 11, 396, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 3, 14, 7, 11, 396, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 3, 15, 55, 40, 240, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id_adress = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgreementsId = table.Column<int>(nullable: false),
                    indoor = table.Column<string>(maxLength: 10, nullable: false),
                    lat = table.Column<string>(maxLength: 20, nullable: true),
                    Lon = table.Column<string>(maxLength: 20, nullable: true),
                    outdoor = table.Column<string>(maxLength: 15, nullable: false),
                    reference = table.Column<string>(maxLength: 200, nullable: false),
                    street = table.Column<string>(maxLength: 150, nullable: false),
                    SuburbsId = table.Column<int>(nullable: true),
                    type_address = table.Column<string>(maxLength: 5, nullable: true),
                    zip = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id_adress);
                    table.ForeignKey(
                        name: "FK_Address_Agreement_AgreementsId",
                        column: x => x.AgreementsId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Address_Suburb_SuburbsId",
                        column: x => x.SuburbsId,
                        principalTable: "Suburb",
                        principalColumn: "id_suburb",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_AgreementsId",
                table: "Address",
                column: "AgreementsId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_SuburbsId",
                table: "Address",
                column: "SuburbsId");
        }
    }
}
