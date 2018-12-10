using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Tariff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "acronym",
                table: "Type_Consume",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "end_consume",
                table: "Tariff",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "have_consume",
                table: "Tariff",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "start_consume",
                table: "Tariff",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TypeConsumeId",
                table: "Tariff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeIntakeId",
                table: "Tariff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "in_agreement",
                table: "Service",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 10, 13, 11, 9, 427, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 8, 12, 0, 24, 195, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Client",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Address",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "System_Parameters",
                columns: table => new
                {
                    id_system_parameters = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    type_column = table.Column<short>(nullable: false),
                    number_column = table.Column<double>(nullable: false),
                    text_column = table.Column<string>(nullable: true),
                    date_column = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Parameters", x => x.id_system_parameters);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeConsumeId",
                table: "Tariff",
                column: "TypeConsumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeIntakeId",
                table: "Tariff",
                column: "TypeIntakeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tariff_Type_Consume_TypeConsumeId",
                table: "Tariff",
                column: "TypeConsumeId",
                principalTable: "Type_Consume",
                principalColumn: "id_type_consume",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tariff_Type_Intake_TypeIntakeId",
                table: "Tariff",
                column: "TypeIntakeId",
                principalTable: "Type_Intake",
                principalColumn: "id_type_intake",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tariff_Type_Consume_TypeConsumeId",
                table: "Tariff");

            migrationBuilder.DropForeignKey(
                name: "FK_Tariff_Type_Intake_TypeIntakeId",
                table: "Tariff");

            migrationBuilder.DropTable(
                name: "System_Parameters");

            migrationBuilder.DropIndex(
                name: "IX_Tariff_TypeConsumeId",
                table: "Tariff");

            migrationBuilder.DropIndex(
                name: "IX_Tariff_TypeIntakeId",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "acronym",
                table: "Type_Consume");

            migrationBuilder.DropColumn(
                name: "end_consume",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "have_consume",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "start_consume",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "TypeConsumeId",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "TypeIntakeId",
                table: "Tariff");

            migrationBuilder.DropColumn(
                name: "in_agreement",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Address");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 8, 12, 0, 24, 195, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 10, 13, 11, 9, 427, DateTimeKind.Local));
        }
    }
}
