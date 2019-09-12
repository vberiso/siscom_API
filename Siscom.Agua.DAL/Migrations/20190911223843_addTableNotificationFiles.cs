using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableNotificationFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "notificationFileId",
                table: "Notification",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 11, 17, 38, 43, 72, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 6, 17, 1, 37, 458, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "notification_files",
                columns: table => new
                {
                    id_Notification_files = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>(nullable: true),
                    user_name = table.Column<string>(nullable: true),
                    generation_date = table.Column<DateTime>(nullable: false),
                    pdf_notifications = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_files", x => x.id_Notification_files);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_files");

            migrationBuilder.DropColumn(
                name: "notificationFileId",
                table: "Notification");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 9, 6, 17, 1, 37, 458, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 9, 11, 17, 38, 43, 72, DateTimeKind.Local));
        }
    }
}
