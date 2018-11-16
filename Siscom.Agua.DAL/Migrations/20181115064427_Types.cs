using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Type_User_TypeUserId",
                table: "Client");

            migrationBuilder.DropTable(
                name: "Type_User");

            migrationBuilder.DropIndex(
                name: "IX_Client_TypeUserId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "TypeUserId",
                table: "Client");

            migrationBuilder.AddColumn<string>(
                name: "type_user",
                table: "Client",
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Group_Type",
                columns: table => new
                {
                    id_group_type = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Type", x => x.id_group_type);
                });

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    id_type = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 5, nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    GroupTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.id_type);
                    table.ForeignKey(
                        name: "FK_Type_Group_Type_GroupTypeId",
                        column: x => x.GroupTypeId,
                        principalTable: "Group_Type",
                        principalColumn: "id_group_type",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Type_GroupTypeId",
                table: "Type",
                column: "GroupTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.DropTable(
                name: "Group_Type");

            migrationBuilder.DropColumn(
                name: "type_user",
                table: "Client");

            migrationBuilder.AddColumn<int>(
                name: "TypeUserId",
                table: "Client",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Type_User",
                columns: table => new
                {
                    id_type_user = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_User", x => x.id_type_user);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_TypeUserId",
                table: "Client",
                column: "TypeUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Type_User_TypeUserId",
                table: "Client",
                column: "TypeUserId",
                principalTable: "Type_User",
                principalColumn: "id_type_user",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
