using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TransitPolice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Ticket_AspNetUsers_UserId",
                table: "Assignment_Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_Ticket_UserId",
                table: "Assignment_Ticket");

            migrationBuilder.DropColumn(
                name: "serie",
                table: "Assignment_Ticket");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Assignment_Ticket");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 14, 13, 9, 20, 576, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 13, 13, 49, 42, 502, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "TransitPoliceId",
                table: "Assignment_Ticket",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TransitPolices",
                columns: table => new
                {
                    id_transit_police = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(maxLength: 50, nullable: true),
                    email = table.Column<string>(maxLength: 150, nullable: true),
                    address = table.Column<string>(maxLength: 300, nullable: true),
                    plate = table.Column<string>(maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitPolices", x => x.id_transit_police);
                    table.ForeignKey(
                        name: "FK_TransitPolices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Ticket_TransitPoliceId",
                table: "Assignment_Ticket",
                column: "TransitPoliceId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitPolices_UserId",
                table: "TransitPolices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Ticket_TransitPolices_TransitPoliceId",
                table: "Assignment_Ticket",
                column: "TransitPoliceId",
                principalTable: "TransitPolices",
                principalColumn: "id_transit_police",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Ticket_TransitPolices_TransitPoliceId",
                table: "Assignment_Ticket");

            migrationBuilder.DropTable(
                name: "TransitPolices");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_Ticket_TransitPoliceId",
                table: "Assignment_Ticket");

            migrationBuilder.DropColumn(
                name: "TransitPoliceId",
                table: "Assignment_Ticket");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 13, 13, 49, 42, 502, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 14, 13, 9, 20, 576, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "serie",
                table: "Assignment_Ticket",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Assignment_Ticket",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Ticket_UserId",
                table: "Assignment_Ticket",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Ticket_AspNetUsers_UserId",
                table: "Assignment_Ticket",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
