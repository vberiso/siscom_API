using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Origin_Payment",
                table: "Origin_Payment",
                newName: "id_origin_payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 26, 11, 32, 5, 401, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 25, 0, 22, 55, 358, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "is_active",
                table: "Contact",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "External_Origin_Payment",
                columns: table => new
                {
                    id_external_origin_payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_External_Origin_Payment", x => x.id_external_origin_payment);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id_payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    payment_date = table.Column<DateTime>(nullable: false),
                    branch_office = table.Column<string>(maxLength: 20, nullable: false),
                    subtotal = table.Column<double>(nullable: false),
                    percentage_tax = table.Column<string>(maxLength: 2, nullable: true),
                    tax = table.Column<double>(nullable: false),
                    total = table.Column<double>(nullable: false),
                    authorization = table.Column<string>(maxLength: 50, nullable: true),
                    debt = table.Column<int>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    OriginPaymentId = table.Column<int>(nullable: true),
                    PayMethodId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.id_payment);
                    table.ForeignKey(
                        name: "FK_Payment_Origin_Payment_OriginPaymentId",
                        column: x => x.OriginPaymentId,
                        principalTable: "Origin_Payment",
                        principalColumn: "id_origin_payment",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_Pay_Method_PayMethodId",
                        column: x => x.PayMethodId,
                        principalTable: "Pay_Method",
                        principalColumn: "id_pay_method",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tariff",
                columns: table => new
                {
                    id_tariff = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    concept = table.Column<string>(maxLength: 80, nullable: false),
                    account_number = table.Column<string>(maxLength: 20, nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    percentage = table.Column<short>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    from_date = table.Column<DateTime>(type: "date", nullable: false),
                    until_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_active = table.Column<int>(nullable: false),
                    TypeIntakeId = table.Column<int>(nullable: true),
                    TypeServiceId = table.Column<int>(nullable: true),
                    TypeUseId = table.Column<int>(nullable: true),
                    ServiceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariff", x => x.id_tariff);
                    table.ForeignKey(
                        name: "FK_Tariff_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tariff_Type_Intake_TypeIntakeId",
                        column: x => x.TypeIntakeId,
                        principalTable: "Type_Intake",
                        principalColumn: "id_type_intake",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tariff_Type_Service_TypeServiceId",
                        column: x => x.TypeServiceId,
                        principalTable: "Type_Service",
                        principalColumn: "id_type_service",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tariff_Type_Use_TypeUseId",
                        column: x => x.TypeUseId,
                        principalTable: "Type_Use",
                        principalColumn: "id_type_use",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OriginPaymentId",
                table: "Payment",
                column: "OriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PayMethodId",
                table: "Payment",
                column: "PayMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_ServiceId",
                table: "Tariff",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeIntakeId",
                table: "Tariff",
                column: "TypeIntakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeServiceId",
                table: "Tariff",
                column: "TypeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariff_TypeUseId",
                table: "Tariff",
                column: "TypeUseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "External_Origin_Payment");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Tariff");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "id_origin_payment",
                table: "Origin_Payment",
                newName: "Origin_Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 25, 0, 22, 55, 358, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 26, 11, 32, 5, 401, DateTimeKind.Local));
        }
    }
}
