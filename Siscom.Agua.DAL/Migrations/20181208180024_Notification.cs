using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Notification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 8, 12, 0, 24, 195, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 7, 16, 29, 45, 978, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    id_notification = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 40, nullable: false),
                    notification_date = table.Column<DateTime>(nullable: false),
                    from_date = table.Column<DateTime>(nullable: false),
                    until_date = table.Column<DateTime>(nullable: false),
                    subtotal = table.Column<double>(nullable: false),
                    tax = table.Column<double>(nullable: false),
                    rounding = table.Column<double>(nullable: false),
                    total = table.Column<double>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.id_notification);
                    table.ForeignKey(
                        name: "FK_Notification_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification_Detail",
                columns: table => new
                {
                    id_notification_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<double>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    code_concept = table.Column<string>(maxLength: 5, nullable: false),
                    name_concept = table.Column<string>(maxLength: 150, nullable: false),
                    NotificationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification_Detail", x => x.id_notification_detail);
                    table.ForeignKey(
                        name: "FK_Notification_Detail_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "id_notification",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AgreementId",
                table: "Notification",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Detail_NotificationId",
                table: "Notification_Detail",
                column: "NotificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification_Detail");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 7, 16, 29, 45, 978, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 8, 12, 0, 24, 195, DateTimeKind.Local));
        }
    }
}
