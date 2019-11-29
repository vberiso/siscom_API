using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class CreateTablesPromotions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.CreateTable(
                name: "PromotionDebt",
                columns: table => new
                {
                    id_promotion_debt = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PromotionId = table.Column<int>(nullable: false),
                    DebtId = table.Column<int>(nullable: false),
                    use = table.Column<string>(nullable: true),
                    use_id = table.Column<string>(nullable: true),
                    DebtApplyPromotion = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionDebt", x => x.id_promotion_debt);
                });

            migrationBuilder.CreateTable(
                name: "PromotionGroup",
                columns: table => new
                {
                    id_promotion_group = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(nullable: true),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionGroup", x => x.id_promotion_group);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    id_promotion = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(nullable: true),
                    is_active = table.Column<bool>(nullable: false),
                    PromotionGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.id_promotion);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.DropTable(
                name: "PromotionDebt");

            migrationBuilder.DropTable(
                name: "PromotionGroup");

            migrationBuilder.DropTable(
                name: "Promotions");

         
        }
    }
}
