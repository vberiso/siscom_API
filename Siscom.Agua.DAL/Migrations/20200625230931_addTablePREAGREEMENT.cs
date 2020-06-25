using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablePREAGREEMENT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 25, 18, 9, 29, 949, DateTimeKind.Local).AddTicks(1264),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 23, 12, 35, 47, 348, DateTimeKind.Local).AddTicks(5629));

            migrationBuilder.CreateTable(
                name: "pre_agreement",
                columns: table => new
                {
                    id_pre_agreement = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 30, nullable: true),
                    TypeIntakeId = table.Column<int>(nullable: false),
                    TypeServiceId = table.Column<int>(nullable: false),
                    TypeUseId = table.Column<int>(nullable: false),
                    TypeClassificationId = table.Column<int>(nullable: false),
                    client_name = table.Column<string>(maxLength: 200, nullable: true),
                    client_last_name = table.Column<string>(maxLength: 80, nullable: true),
                    client_second_last_name = table.Column<string>(maxLength: 80, nullable: true),
                    street = table.Column<string>(maxLength: 150, nullable: true),
                    outdoor = table.Column<string>(maxLength: 50, nullable: true),
                    indoor = table.Column<string>(maxLength: 50, nullable: true),
                    zip = table.Column<string>(maxLength: 5, nullable: true),
                    reference = table.Column<string>(maxLength: 200, nullable: true),
                    lat = table.Column<string>(maxLength: 20, nullable: true),
                    Lon = table.Column<string>(maxLength: 20, nullable: true),
                    SuburbsId = table.Column<int>(nullable: false),
                    service_id_1 = table.Column<int>(nullable: false),
                    service_id_2 = table.Column<int>(nullable: false),
                    service_id_3 = table.Column<int>(nullable: false),
                    registration_reason = table.Column<string>(nullable: true),
                    observation = table.Column<string>(nullable: true),
                    status = table.Column<string>(maxLength: 6, nullable: true),
                    date_registration = table.Column<DateTime>(nullable: false),
                    OrderWorkId = table.Column<int>(nullable: false),
                    folio_order_result = table.Column<string>(nullable: true),
                    agreementId_new = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_agreement", x => x.id_pre_agreement);
                    table.ForeignKey(
                        name: "FK_pre_agreement_Suburb_SuburbsId",
                        column: x => x.SuburbsId,
                        principalTable: "Suburb",
                        principalColumn: "id_suburb",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pre_agreement_Type_Classification_TypeClassificationId",
                        column: x => x.TypeClassificationId,
                        principalTable: "Type_Classification",
                        principalColumn: "id_type_classification",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pre_agreement_Type_Intake_TypeIntakeId",
                        column: x => x.TypeIntakeId,
                        principalTable: "Type_Intake",
                        principalColumn: "id_type_intake",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pre_agreement_Type_Service_TypeServiceId",
                        column: x => x.TypeServiceId,
                        principalTable: "Type_Service",
                        principalColumn: "id_type_service",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pre_agreement_Type_Use_TypeUseId",
                        column: x => x.TypeUseId,
                        principalTable: "Type_Use",
                        principalColumn: "id_type_use",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pre_agreement_SuburbsId",
                table: "pre_agreement",
                column: "SuburbsId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_agreement_TypeClassificationId",
                table: "pre_agreement",
                column: "TypeClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_agreement_TypeIntakeId",
                table: "pre_agreement",
                column: "TypeIntakeId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_agreement_TypeServiceId",
                table: "pre_agreement",
                column: "TypeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_agreement_TypeUseId",
                table: "pre_agreement",
                column: "TypeUseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pre_agreement");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 23, 12, 35, 47, 348, DateTimeKind.Local).AddTicks(5629),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 25, 18, 9, 29, 949, DateTimeKind.Local).AddTicks(1264));
        }
    }
}
