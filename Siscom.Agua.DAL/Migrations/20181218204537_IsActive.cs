using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class IsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Use",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Transaction",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_State_Service",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Service",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Regime",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Period",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Intake",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Consume",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Commertial_Business",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Type_Classification",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "Region",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Pay_Method",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Origin_Payment",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 18, 14, 45, 37, 435, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 17, 13, 24, 37, 392, DateTimeKind.Local));

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "External_Origin_Payment",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Diameter",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expiration_date",
                table: "Debt_Period",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "expiration_date",
                table: "Debt",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "INPC",
                columns: table => new
                {
                    id_inpc = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    year = table.Column<short>(nullable: false),
                    month = table.Column<short>(nullable: false),
                    value = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INPC", x => x.id_inpc);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "INPC");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Use");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Transaction");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_State_Service");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Service");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Regime");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Period");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Intake");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Consume");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Commertial_Business");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Type_Classification");

            migrationBuilder.DropColumn(
                name: "price",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Pay_Method");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Origin_Payment");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "External_Origin_Payment");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Diameter");

            migrationBuilder.DropColumn(
                name: "expiration_date",
                table: "Debt_Period");

            migrationBuilder.DropColumn(
                name: "expiration_date",
                table: "Debt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 17, 13, 24, 37, 392, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 18, 14, 45, 37, 435, DateTimeKind.Local));
        }
    }
}
