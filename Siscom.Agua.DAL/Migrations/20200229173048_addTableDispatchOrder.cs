using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addTableDispatchOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 29, 11, 30, 47, 107, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 10, 13, 8, 35, 268, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "DispatchOrder",
                columns: table => new
                {
                    id_dispatch_order = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    orderworkid = table.Column<int>(nullable: false),
                    technicalstaffid = table.Column<int>(nullable: false),
                    IMEI = table.Column<string>(maxLength: 50, nullable: true),
                    date_asign = table.Column<DateTime>(nullable: false),
                    date_attended = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchOrder", x => x.id_dispatch_order);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchOrder");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 2, 10, 13, 8, 35, 268, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 2, 29, 11, 30, 47, 107, DateTimeKind.Local));
        }
    }
}
