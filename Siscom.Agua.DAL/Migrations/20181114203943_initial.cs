using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branch_Office",
                columns: table => new
                {
                    id_branch_office = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    open = table.Column<DateTime>(nullable: false),
                    close = table.Column<DateTime>(nullable: false),
                    dont_close = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch_Office", x => x.id_branch_office);
                });

            migrationBuilder.CreateTable(
                name: "Clasification",
                columns: table => new
                {
                    id_clasification = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clasification", x => x.id_clasification);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    id_country = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    abbreviation = table.Column<string>(maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.id_country);
                });

            migrationBuilder.CreateTable(
                name: "Diameter",
                columns: table => new
                {
                    id_diameter = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 5, nullable: false),
                    description = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diameter", x => x.id_diameter);
                });

            migrationBuilder.CreateTable(
                name: "Pay_Method",
                columns: table => new
                {
                    id_pay_method = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pay_Method", x => x.id_pay_method);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    id_region = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.id_region);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    id_service = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.id_service);
                });

            migrationBuilder.CreateTable(
                name: "Type_Commertial_Business",
                columns: table => new
                {
                    id_type_commertial_business = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    clasification_group = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Commertial_Business", x => x.id_type_commertial_business);
                });

            migrationBuilder.CreateTable(
                name: "Type_Consume",
                columns: table => new
                {
                    id_type_consume = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Consume", x => x.id_type_consume);
                });

            migrationBuilder.CreateTable(
                name: "Type_Intake",
                columns: table => new
                {
                    id_type_intake = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Intake", x => x.id_type_intake);
                });

            migrationBuilder.CreateTable(
                name: "Type_Period",
                columns: table => new
                {
                    id_type_period = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Period", x => x.id_type_period);
                });

            migrationBuilder.CreateTable(
                name: "Type_Regime",
                columns: table => new
                {
                    id_type_regime = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Regime", x => x.id_type_regime);
                });

            migrationBuilder.CreateTable(
                name: "Type_Service",
                columns: table => new
                {
                    id_type_service = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Service", x => x.id_type_service);
                });

            migrationBuilder.CreateTable(
                name: "Type_State_Service",
                columns: table => new
                {
                    id_type_state_service = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_State_Service", x => x.id_type_state_service);
                });

            migrationBuilder.CreateTable(
                name: "Type_Transaction",
                columns: table => new
                {
                    id_type_transaction = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Transaction", x => x.id_type_transaction);
                });

            migrationBuilder.CreateTable(
                name: "Type_Use",
                columns: table => new
                {
                    id_type_use = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Use", x => x.id_type_use);
                });

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

            migrationBuilder.CreateTable(
                name: "View",
                columns: table => new
                {
                    id_view = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    alias = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View", x => x.id_view);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folio",
                columns: table => new
                {
                    id_folio = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    initial = table.Column<int>(nullable: false),
                    secuential = table.Column<int>(nullable: false),
                    BranchOfficeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folio", x => x.id_folio);
                    table.ForeignKey(
                        name: "FK_Folio_Branch_Office_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "Branch_Office",
                        principalColumn: "id_branch_office",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Terminal",
                columns: table => new
                {
                    id_terminal = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    mac_adress = table.Column<string>(maxLength: 20, nullable: false),
                    cash_box = table.Column<decimal>(nullable: false),
                    BranchOfficeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal", x => x.id_terminal);
                    table.ForeignKey(
                        name: "FK_Terminal_Branch_Office_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "Branch_Office",
                        principalColumn: "id_branch_office",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    id_state = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    CountriesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.id_state);
                    table.ForeignKey(
                        name: "FK_State_Country_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Country",
                        principalColumn: "id_country",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agreement",
                columns: table => new
                {
                    id_agreement = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    account = table.Column<string>(maxLength: 50, nullable: false),
                    account_date = table.Column<DateTime>(nullable: false),
                    derivatives = table.Column<int>(nullable: false),
                    TypeServiceId = table.Column<int>(nullable: true),
                    TypeUseId = table.Column<int>(nullable: true),
                    TypeConsumeId = table.Column<int>(nullable: true),
                    TypeRegimeId = table.Column<int>(nullable: true),
                    TypePeriodId = table.Column<int>(nullable: true),
                    TypeCommertialBusinessId = table.Column<int>(nullable: true),
                    TypeStateServiceId = table.Column<int>(nullable: true),
                    DiameterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement", x => x.id_agreement);
                    table.ForeignKey(
                        name: "FK_Agreement_Diameter_DiameterId",
                        column: x => x.DiameterId,
                        principalTable: "Diameter",
                        principalColumn: "id_diameter",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Commertial_Business_TypeCommertialBusinessId",
                        column: x => x.TypeCommertialBusinessId,
                        principalTable: "Type_Commertial_Business",
                        principalColumn: "id_type_commertial_business",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Consume_TypeConsumeId",
                        column: x => x.TypeConsumeId,
                        principalTable: "Type_Consume",
                        principalColumn: "id_type_consume",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Period_TypePeriodId",
                        column: x => x.TypePeriodId,
                        principalTable: "Type_Period",
                        principalColumn: "id_type_period",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Regime_TypeRegimeId",
                        column: x => x.TypeRegimeId,
                        principalTable: "Type_Regime",
                        principalColumn: "id_type_regime",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Service_TypeServiceId",
                        column: x => x.TypeServiceId,
                        principalTable: "Type_Service",
                        principalColumn: "id_type_service",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_State_Service_TypeStateServiceId",
                        column: x => x.TypeStateServiceId,
                        principalTable: "Type_State_Service",
                        principalColumn: "id_type_state_service",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Use_TypeUseId",
                        column: x => x.TypeUseId,
                        principalTable: "Type_Use",
                        principalColumn: "id_type_use",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Control",
                columns: table => new
                {
                    id_control = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    alias = table.Column<string>(maxLength: 50, nullable: false),
                    ViewId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control", x => x.id_control);
                    table.ForeignKey(
                        name: "FK_Control_View_ViewId",
                        column: x => x.ViewId,
                        principalTable: "View",
                        principalColumn: "id_view",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "View_Profile",
                columns: table => new
                {
                    id_profile = table.Column<int>(nullable: false),
                    id_view = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_Profile", x => new { x.id_profile, x.id_view });
                    table.ForeignKey(
                        name: "FK_View_Profile_AspNetRoles_id_view",
                        column: x => x.id_view,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_View_Profile_View_id_profile",
                        column: x => x.id_profile,
                        principalTable: "View",
                        principalColumn: "id_view",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terminal_User",
                columns: table => new
                {
                    open_date = table.Column<DateTime>(nullable: false),
                    id_terminal = table.Column<int>(nullable: false),
                    id_user = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal_User", x => new { x.id_terminal, x.id_user });
                    table.ForeignKey(
                        name: "FK_Terminal_User_Terminal_id_terminal",
                        column: x => x.id_terminal,
                        principalTable: "Terminal",
                        principalColumn: "id_terminal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Terminal_User_AspNetUsers_id_user",
                        column: x => x.id_user,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Town",
                columns: table => new
                {
                    id_town = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    StatesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Town", x => x.id_town);
                    table.ForeignKey(
                        name: "FK_Town_State_StatesId",
                        column: x => x.StatesId,
                        principalTable: "State",
                        principalColumn: "id_state",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agreement_Service",
                columns: table => new
                {
                    id_service = table.Column<int>(nullable: false),
                    id_agreement = table.Column<int>(nullable: false),
                    agreement_date = table.Column<DateTime>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_Service", x => new { x.id_service, x.id_agreement });
                    table.ForeignKey(
                        name: "FK_Agreement_Service_Agreement_id_agreement",
                        column: x => x.id_agreement,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Service_Service_id_service",
                        column: x => x.id_service,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    id_client = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    last_name = table.Column<string>(maxLength: 80, nullable: false),
                    second_last_name = table.Column<string>(maxLength: 80, nullable: false),
                    rfc = table.Column<string>(maxLength: 13, nullable: false),
                    curp = table.Column<string>(maxLength: 18, nullable: true),
                    ine = table.Column<string>(maxLength: 13, nullable: false),
                    email = table.Column<string>(maxLength: 150, nullable: true),
                    TypeUserId = table.Column<int>(nullable: true),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.id_client);
                    table.ForeignKey(
                        name: "FK_Client_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_Type_User_TypeUserId",
                        column: x => x.TypeUserId,
                        principalTable: "Type_User",
                        principalColumn: "id_type_user",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    id_transaction = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 8, nullable: true),
                    date_transaction = table.Column<DateTime>(nullable: false),
                    sign = table.Column<bool>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    aplication = table.Column<string>(maxLength: 20, nullable: false),
                    TerminalUserTermianlId = table.Column<int>(nullable: true),
                    TerminalUserUserId = table.Column<string>(nullable: true),
                    TypeTransactionId = table.Column<int>(nullable: true),
                    PayMethodId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.id_transaction);
                    table.ForeignKey(
                        name: "FK_Transaction_Pay_Method_PayMethodId",
                        column: x => x.PayMethodId,
                        principalTable: "Pay_Method",
                        principalColumn: "id_pay_method",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Type_Transaction_TypeTransactionId",
                        column: x => x.TypeTransactionId,
                        principalTable: "Type_Transaction",
                        principalColumn: "id_type_transaction",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Terminal_User_TerminalUserTermianlId_TerminalUserUserId",
                        columns: x => new { x.TerminalUserTermianlId, x.TerminalUserUserId },
                        principalTable: "Terminal_User",
                        principalColumns: new[] { "id_terminal", "id_user" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suburb",
                columns: table => new
                {
                    id_suburb = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    TownsId = table.Column<int>(nullable: true),
                    RegionsId = table.Column<int>(nullable: true),
                    ClasificationsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suburb", x => x.id_suburb);
                    table.ForeignKey(
                        name: "FK_Suburb_Clasification_ClasificationsId",
                        column: x => x.ClasificationsId,
                        principalTable: "Clasification",
                        principalColumn: "id_clasification",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suburb_Region_RegionsId",
                        column: x => x.RegionsId,
                        principalTable: "Region",
                        principalColumn: "id_region",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suburb_Town_TownsId",
                        column: x => x.TownsId,
                        principalTable: "Town",
                        principalColumn: "id_town",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    id_contact = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    phone_number = table.Column<string>(maxLength: 50, nullable: false),
                    is_movil = table.Column<bool>(nullable: false),
                    ClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.id_contact);
                    table.ForeignKey(
                        name: "FK_Contact_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "id_client",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id_adress = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    street = table.Column<string>(maxLength: 150, nullable: false),
                    outdoor = table.Column<string>(maxLength: 15, nullable: false),
                    indoor = table.Column<string>(maxLength: 10, nullable: false),
                    zip = table.Column<string>(maxLength: 5, nullable: false),
                    reference = table.Column<string>(maxLength: 200, nullable: false),
                    lat = table.Column<string>(maxLength: 12, nullable: true),
                    Lon = table.Column<string>(maxLength: 12, nullable: true),
                    type_address = table.Column<byte>(nullable: false),
                    AgreementsId = table.Column<int>(nullable: false),
                    SuburbsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id_adress);
                    table.ForeignKey(
                        name: "FK_Address_Agreement_AgreementsId",
                        column: x => x.AgreementsId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Address_Suburb_SuburbsId",
                        column: x => x.SuburbsId,
                        principalTable: "Suburb",
                        principalColumn: "id_suburb",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_AgreementsId",
                table: "Address",
                column: "AgreementsId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_SuburbsId",
                table: "Address",
                column: "SuburbsId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_DiameterId",
                table: "Agreement",
                column: "DiameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeCommertialBusinessId",
                table: "Agreement",
                column: "TypeCommertialBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeConsumeId",
                table: "Agreement",
                column: "TypeConsumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypePeriodId",
                table: "Agreement",
                column: "TypePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeRegimeId",
                table: "Agreement",
                column: "TypeRegimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeServiceId",
                table: "Agreement",
                column: "TypeServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeStateServiceId",
                table: "Agreement",
                column: "TypeStateServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_TypeUseId",
                table: "Agreement",
                column: "TypeUseId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Service_id_agreement",
                table: "Agreement_Service",
                column: "id_agreement");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Client_AgreementId",
                table: "Client",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_TypeUserId",
                table: "Client",
                column: "TypeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ClientId",
                table: "Contact",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Control_ViewId",
                table: "Control",
                column: "ViewId");

            migrationBuilder.CreateIndex(
                name: "IX_Folio_BranchOfficeId",
                table: "Folio",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Folio_initial",
                table: "Folio",
                column: "initial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_State_CountriesId",
                table: "State",
                column: "CountriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Suburb_ClasificationsId",
                table: "Suburb",
                column: "ClasificationsId");

            migrationBuilder.CreateIndex(
                name: "IX_Suburb_RegionsId",
                table: "Suburb",
                column: "RegionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Suburb_TownsId",
                table: "Suburb",
                column: "TownsId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_BranchOfficeId",
                table: "Terminal",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_User_id_user",
                table: "Terminal_User",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_Town_StatesId",
                table: "Town",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PayMethodId",
                table: "Transaction",
                column: "PayMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TypeTransactionId",
                table: "Transaction",
                column: "TypeTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction",
                columns: new[] { "TerminalUserTermianlId", "TerminalUserUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_View_Profile_id_view",
                table: "View_Profile",
                column: "id_view");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Agreement_Service");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Control");

            migrationBuilder.DropTable(
                name: "Folio");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Type_Intake");

            migrationBuilder.DropTable(
                name: "View_Profile");

            migrationBuilder.DropTable(
                name: "Suburb");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Pay_Method");

            migrationBuilder.DropTable(
                name: "Type_Transaction");

            migrationBuilder.DropTable(
                name: "Terminal_User");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "View");

            migrationBuilder.DropTable(
                name: "Clasification");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Town");

            migrationBuilder.DropTable(
                name: "Agreement");

            migrationBuilder.DropTable(
                name: "Type_User");

            migrationBuilder.DropTable(
                name: "Terminal");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "Diameter");

            migrationBuilder.DropTable(
                name: "Type_Commertial_Business");

            migrationBuilder.DropTable(
                name: "Type_Consume");

            migrationBuilder.DropTable(
                name: "Type_Period");

            migrationBuilder.DropTable(
                name: "Type_Regime");

            migrationBuilder.DropTable(
                name: "Type_Service");

            migrationBuilder.DropTable(
                name: "Type_State_Service");

            migrationBuilder.DropTable(
                name: "Type_Use");

            migrationBuilder.DropTable(
                name: "Branch_Office");

            migrationBuilder.DropTable(
                name: "Country");
        }
    }
}
