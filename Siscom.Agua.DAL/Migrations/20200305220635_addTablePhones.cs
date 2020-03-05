using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTablePhones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 5, 16, 6, 33, 884, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 3, 13, 1, 5, 311, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "phone",
                columns: table => new
                {
                    id_Phone = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_device = table.Column<string>(maxLength: 30, nullable: true),
                    phone_number = table.Column<string>(maxLength: 20, nullable: true),
                    assigned_user = table.Column<string>(maxLength: 30, nullable: true),
                    is_active = table.Column<bool>(nullable: false),
                    register_date = table.Column<DateTime>(nullable: false),
                    last_update_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone", x => x.id_Phone);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "phone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 3, 3, 13, 1, 5, 311, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 3, 5, 16, 6, 33, 884, DateTimeKind.Local));
        }
    }
}
