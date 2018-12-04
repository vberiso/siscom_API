using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    id_account = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    prefix = table.Column<string>(nullable: true),
                    secuential = table.Column<int>(nullable: false),
                    suffixes = table.Column<string>(nullable: true),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.id_account);
                });

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
                    opening = table.Column<DateTime>(nullable: false),
                    closing = table.Column<DateTime>(nullable: false),
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
                name: "Discount",
                columns: table => new
                {
                    id_discount = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    percentage = table.Column<short>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    TypePeriodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.id_discount);
                });

            migrationBuilder.CreateTable(
                name: "External_Origin_Payment",
                columns: table => new
                {
                    id_external_origin_payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 15, nullable: false),
                    is_bank = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_External_Origin_Payment", x => x.id_external_origin_payment);
                });

            migrationBuilder.CreateTable(
                name: "Group_Status",
                columns: table => new
                {
                    id_group_type = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Status", x => x.id_group_type);
                });

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
                name: "Origin_Payment",
                columns: table => new
                {
                    id_origin_payment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Origin_Payment", x => x.id_origin_payment);
                });

            migrationBuilder.CreateTable(
                name: "Pay_Method",
                columns: table => new
                {
                    id_pay_method = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false)
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
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    order = table.Column<short>(nullable: false),
                    is_service = table.Column<bool>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    in_agreement = table.Column<bool>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.id_service);
                });

            migrationBuilder.CreateTable(
                name: "System_Log",
                columns: table => new
                {
                    id_system_log = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date_log = table.Column<DateTime>(nullable: false),
                    controller = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    parameter = table.Column<string>(nullable: false),
                    action = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Log", x => x.id_system_log);
                });

            migrationBuilder.CreateTable(
                name: "Type_Commertial_Business",
                columns: table => new
                {
                    id_type_commertial_business = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    clasification_group = table.Column<int>(nullable: false),
                    intake_acronym = table.Column<string>(maxLength: 2, nullable: false)
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
                    name = table.Column<string>(maxLength: 20, nullable: false),
                    acronym = table.Column<string>(maxLength: 2, nullable: false)
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
                    name = table.Column<string>(maxLength: 15, nullable: false),
                    mounth = table.Column<short>(nullable: false)
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
                    name = table.Column<string>(maxLength: 20, nullable: false),
                    intake_acronym = table.Column<string>(maxLength: 2, nullable: false)
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
                    name = table.Column<string>(maxLength: 50, nullable: false)
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
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    intake_acronym = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type_Use", x => x.id_type_use);
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
                    range = table.Column<string>(nullable: false),
                    initial = table.Column<int>(nullable: false),
                    secuential = table.Column<int>(nullable: false),
                    date_current = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2018, 12, 4, 16, 5, 0, 728, DateTimeKind.Local)),
                    is_active = table.Column<int>(nullable: false),
                    BranchOfficeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folio", x => x.id_folio);
                    table.ForeignKey(
                        name: "FK_Folio_Branch_Office_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "Branch_Office",
                        principalColumn: "id_branch_office",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terminal",
                columns: table => new
                {
                    id_terminal = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    mac_adress = table.Column<string>(maxLength: 20, nullable: false),
                    cash_box = table.Column<double>(nullable: false),
                    is_active = table.Column<bool>(nullable: false, defaultValue: true),
                    BranchOfficeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal", x => x.id_terminal);
                    table.ForeignKey(
                        name: "FK_Terminal_Branch_Office_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "Branch_Office",
                        principalColumn: "id_branch_office",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    id_state = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    CountriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.id_state);
                    table.ForeignKey(
                        name: "FK_State_Country_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Country",
                        principalColumn: "id_country",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    id_status = table.Column<string>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    GroupStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => new { x.id_status, x.GroupStatusId });
                    table.UniqueConstraint("AK_Status_id_status", x => x.id_status);
                    table.ForeignKey(
                        name: "FK_Status_Group_Status_GroupStatusId",
                        column: x => x.GroupStatusId,
                        principalTable: "Group_Status",
                        principalColumn: "id_group_type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    id_type = table.Column<string>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    GroupTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.id_type);
                    table.ForeignKey(
                        name: "FK_Type_Group_Type_GroupTypeId",
                        column: x => x.GroupTypeId,
                        principalTable: "Group_Type",
                        principalColumn: "id_group_type",
                        onDelete: ReferentialAction.Cascade);
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
                    rounding = table.Column<double>(nullable: false),
                    total = table.Column<double>(nullable: false),
                    authorization_origin_payment = table.Column<string>(maxLength: 50, nullable: true),
                    transaction_folio = table.Column<string>(maxLength: 40, nullable: true),
                    id_debt = table.Column<int>(nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    OriginPaymentId = table.Column<int>(nullable: false),
                    ExternalOriginPaymentId = table.Column<int>(nullable: false),
                    PayMethodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.id_payment);
                    table.ForeignKey(
                        name: "FK_Payment_External_Origin_Payment_ExternalOriginPaymentId",
                        column: x => x.ExternalOriginPaymentId,
                        principalTable: "External_Origin_Payment",
                        principalColumn: "id_external_origin_payment",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Origin_Payment_OriginPaymentId",
                        column: x => x.OriginPaymentId,
                        principalTable: "Origin_Payment",
                        principalColumn: "id_origin_payment",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Pay_Method_PayMethodId",
                        column: x => x.PayMethodId,
                        principalTable: "Pay_Method",
                        principalColumn: "id_pay_method",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debt_Period",
                columns: table => new
                {
                    id_debt_period = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    period = table.Column<short>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    run_date = table.Column<DateTime>(type: "date", nullable: false),
                    run_hour = table.Column<TimeSpan>(type: "time", nullable: false),
                    TypePeriodId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Period", x => x.id_debt_period);
                    table.ForeignKey(
                        name: "FK_Debt_Period_Type_Period_TypePeriodId",
                        column: x => x.TypePeriodId,
                        principalTable: "Type_Period",
                        principalColumn: "id_type_period",
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
                    num_derivatives = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    type_agreement = table.Column<string>(maxLength: 5, nullable: false),
                    TypeServiceId = table.Column<int>(nullable: false),
                    TypeUseId = table.Column<int>(nullable: false),
                    TypeConsumeId = table.Column<int>(nullable: false),
                    TypeRegimeId = table.Column<int>(nullable: false),
                    TypePeriodId = table.Column<int>(nullable: false),
                    TypeCommertialBusinessId = table.Column<int>(nullable: false),
                    TypeStateServiceId = table.Column<int>(nullable: false),
                    TypeIntakeId = table.Column<int>(nullable: false),
                    DiameterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement", x => x.id_agreement);
                    table.ForeignKey(
                        name: "FK_Agreement_Diameter_DiameterId",
                        column: x => x.DiameterId,
                        principalTable: "Diameter",
                        principalColumn: "id_diameter",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Commertial_Business_TypeCommertialBusinessId",
                        column: x => x.TypeCommertialBusinessId,
                        principalTable: "Type_Commertial_Business",
                        principalColumn: "id_type_commertial_business",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Consume_TypeConsumeId",
                        column: x => x.TypeConsumeId,
                        principalTable: "Type_Consume",
                        principalColumn: "id_type_consume",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Intake_TypeIntakeId",
                        column: x => x.TypeIntakeId,
                        principalTable: "Type_Intake",
                        principalColumn: "id_type_intake",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Period_TypePeriodId",
                        column: x => x.TypePeriodId,
                        principalTable: "Type_Period",
                        principalColumn: "id_type_period",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Regime_TypeRegimeId",
                        column: x => x.TypeRegimeId,
                        principalTable: "Type_Regime",
                        principalColumn: "id_type_regime",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Service_TypeServiceId",
                        column: x => x.TypeServiceId,
                        principalTable: "Type_Service",
                        principalColumn: "id_type_service",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_State_Service_TypeStateServiceId",
                        column: x => x.TypeStateServiceId,
                        principalTable: "Type_State_Service",
                        principalColumn: "id_type_state_service",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Type_Use_TypeUseId",
                        column: x => x.TypeUseId,
                        principalTable: "Type_Use",
                        principalColumn: "id_type_use",
                        onDelete: ReferentialAction.Cascade);
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
                    TypeIntakeId = table.Column<int>(nullable: false),
                    TypeServiceId = table.Column<int>(nullable: false),
                    TypeUseId = table.Column<int>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariff", x => x.id_tariff);
                    table.ForeignKey(
                        name: "FK_Tariff_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tariff_Type_Intake_TypeIntakeId",
                        column: x => x.TypeIntakeId,
                        principalTable: "Type_Intake",
                        principalColumn: "id_type_intake",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tariff_Type_Service_TypeServiceId",
                        column: x => x.TypeServiceId,
                        principalTable: "Type_Service",
                        principalColumn: "id_type_service",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tariff_Type_Use_TypeUseId",
                        column: x => x.TypeUseId,
                        principalTable: "Type_Use",
                        principalColumn: "id_type_use",
                        onDelete: ReferentialAction.Cascade);
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
                    id_terminal_user = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    open_date = table.Column<DateTime>(type: "date", nullable: false),
                    in_operation = table.Column<bool>(nullable: false, defaultValue: false),
                    TerminalId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal_User", x => x.id_terminal_user);
                    table.ForeignKey(
                        name: "FK_Terminal_User_Terminal_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminal",
                        principalColumn: "id_terminal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Terminal_User_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Town",
                columns: table => new
                {
                    id_town = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    StateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Town", x => x.id_town);
                    table.ForeignKey(
                        name: "FK_Town_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "id_state",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agreement_Discount",
                columns: table => new
                {
                    id_discount = table.Column<int>(nullable: false),
                    id_agreement = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_Discount", x => new { x.id_discount, x.id_agreement });
                    table.ForeignKey(
                        name: "FK_Agreement_Discount_Agreement_id_agreement",
                        column: x => x.id_agreement,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Discount_Discount_id_discount",
                        column: x => x.id_discount,
                        principalTable: "Discount",
                        principalColumn: "id_discount",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agreement_Log",
                columns: table => new
                {
                    id_agreement_log = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    agreement_log_date = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(maxLength: 30, nullable: false),
                    observation = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement_Log", x => x.id_agreement_log);
                    table.ForeignKey(
                        name: "FK_Agreement_Log_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agreement_Log_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
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
                    name = table.Column<string>(maxLength: 200, nullable: false),
                    last_name = table.Column<string>(maxLength: 80, nullable: false),
                    second_last_name = table.Column<string>(maxLength: 80, nullable: false),
                    rfc = table.Column<string>(maxLength: 17, nullable: true),
                    curp = table.Column<string>(maxLength: 18, nullable: true),
                    ine = table.Column<string>(maxLength: 20, nullable: true),
                    email = table.Column<string>(maxLength: 150, nullable: true),
                    type_user = table.Column<string>(maxLength: 5, nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "Debt",
                columns: table => new
                {
                    id_debt = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    debit_date = table.Column<DateTime>(nullable: false),
                    from_date = table.Column<DateTime>(type: "date", nullable: false),
                    until_date = table.Column<DateTime>(type: "date", nullable: false),
                    derivatives = table.Column<int>(nullable: false),
                    type_intake = table.Column<string>(maxLength: 50, nullable: false),
                    type_service = table.Column<string>(maxLength: 50, nullable: false),
                    consumption = table.Column<string>(maxLength: 10, nullable: false),
                    discount = table.Column<string>(maxLength: 50, nullable: true),
                    amount = table.Column<double>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    year = table.Column<short>(nullable: false),
                    type = table.Column<string>(maxLength: 5, nullable: false),
                    status = table.Column<string>(maxLength: 5, nullable: false),
                    DebtPeriodId = table.Column<int>(nullable: false),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt", x => x.id_debt);
                    table.ForeignKey(
                        name: "FK_Debt_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Debt_Period_DebtPeriodId",
                        column: x => x.DebtPeriodId,
                        principalTable: "Debt_Period",
                        principalColumn: "id_debt_period",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Derivative",
                columns: table => new
                {
                    id_derivative = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(nullable: false),
                    AgreementId = table.Column<int>(nullable: false),
                    AgreementDerivative = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derivative", x => x.id_derivative);
                    table.ForeignKey(
                        name: "FK_Derivative_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meter",
                columns: table => new
                {
                    id_meter = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    brand = table.Column<string>(maxLength: 50, nullable: false),
                    model = table.Column<string>(maxLength: 50, nullable: false),
                    consumption = table.Column<string>(maxLength: 10, nullable: true),
                    install_date = table.Column<DateTime>(type: "date", nullable: false),
                    deinstall_date = table.Column<DateTime>(type: "date", nullable: false),
                    serial = table.Column<string>(maxLength: 20, nullable: false),
                    wheels = table.Column<string>(maxLength: 1, nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    AgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meter", x => x.id_meter);
                    table.ForeignKey(
                        name: "FK_Meter_Agreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "Agreement",
                        principalColumn: "id_agreement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    id_transaction = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(maxLength: 40, nullable: true),
                    date_transaction = table.Column<DateTime>(nullable: false),
                    sign = table.Column<bool>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    tax = table.Column<double>(nullable: false),
                    rounding = table.Column<double>(nullable: false),
                    aplication = table.Column<string>(maxLength: 20, nullable: false),
                    cancellation_folio = table.Column<string>(maxLength: 40, nullable: true),
                    authorization_origin_payment = table.Column<string>(maxLength: 50, nullable: true),
                    TerminalUserId = table.Column<int>(nullable: false),
                    TypeTransactionId = table.Column<int>(nullable: false),
                    PayMethodId = table.Column<int>(nullable: false),
                    OriginPaymentId = table.Column<int>(nullable: false),
                    ExternalOriginPaymentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.id_transaction);
                    table.ForeignKey(
                        name: "FK_Transaction_External_Origin_Payment_ExternalOriginPaymentId",
                        column: x => x.ExternalOriginPaymentId,
                        principalTable: "External_Origin_Payment",
                        principalColumn: "id_external_origin_payment",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Origin_Payment_OriginPaymentId",
                        column: x => x.OriginPaymentId,
                        principalTable: "Origin_Payment",
                        principalColumn: "id_origin_payment",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Pay_Method_PayMethodId",
                        column: x => x.PayMethodId,
                        principalTable: "Pay_Method",
                        principalColumn: "id_pay_method",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Terminal_User_TerminalUserId",
                        column: x => x.TerminalUserId,
                        principalTable: "Terminal_User",
                        principalColumn: "id_terminal_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Type_Transaction_TypeTransactionId",
                        column: x => x.TypeTransactionId,
                        principalTable: "Type_Transaction",
                        principalColumn: "id_type_transaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suburb",
                columns: table => new
                {
                    id_suburb = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    TownsId = table.Column<int>(nullable: false),
                    RegionsId = table.Column<int>(nullable: false),
                    ClasificationsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suburb", x => x.id_suburb);
                    table.ForeignKey(
                        name: "FK_Suburb_Clasification_ClasificationsId",
                        column: x => x.ClasificationsId,
                        principalTable: "Clasification",
                        principalColumn: "id_clasification",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suburb_Region_RegionsId",
                        column: x => x.RegionsId,
                        principalTable: "Region",
                        principalColumn: "id_region",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suburb_Town_TownsId",
                        column: x => x.TownsId,
                        principalTable: "Town",
                        principalColumn: "id_town",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    id_contact = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    phone_number = table.Column<string>(maxLength: 50, nullable: false),
                    type_number = table.Column<string>(nullable: false),
                    is_active = table.Column<int>(nullable: false),
                    ClienteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.id_contact);
                    table.ForeignKey(
                        name: "FK_Contact_Client_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Client",
                        principalColumn: "id_client",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debt_Detail",
                columns: table => new
                {
                    amount = table.Column<double>(nullable: false),
                    on_account = table.Column<double>(nullable: false),
                    have_tax = table.Column<bool>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false),
                    DebtId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Detail", x => new { x.DebtId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_Debt_Detail_Debt_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debt",
                        principalColumn: "id_debt",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Detail_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "id_service",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consumption",
                columns: table => new
                {
                    id_consumption = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    consumption_date = table.Column<DateTime>(nullable: false),
                    previous_consumption = table.Column<double>(nullable: false),
                    current_consumption = table.Column<double>(nullable: false),
                    consumption = table.Column<double>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    DebtId = table.Column<int>(nullable: false),
                    in_debt = table.Column<bool>(nullable: false),
                    MeterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumption", x => x.id_consumption);
                    table.ForeignKey(
                        name: "FK_Consumption_Meter_MeterId",
                        column: x => x.MeterId,
                        principalTable: "Meter",
                        principalColumn: "id_meter",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction_Detail",
                columns: table => new
                {
                    id_transaction_detail = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    code_concept = table.Column<string>(maxLength: 10, nullable: true),
                    description = table.Column<string>(maxLength: 30, nullable: true),
                    amount = table.Column<double>(nullable: false),
                    TransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction_Detail", x => x.id_transaction_detail);
                    table.ForeignKey(
                        name: "FK_Transaction_Detail_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "id_transaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction_Folio",
                columns: table => new
                {
                    folio = table.Column<string>(nullable: false),
                    DatePrint = table.Column<DateTime>(nullable: false),
                    TransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction_Folio", x => x.folio);
                    table.ForeignKey(
                        name: "FK_Transaction_Folio_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "id_transaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id_address = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    street = table.Column<string>(maxLength: 150, nullable: true),
                    outdoor = table.Column<string>(maxLength: 50, nullable: false),
                    indoor = table.Column<string>(maxLength: 50, nullable: true),
                    zip = table.Column<string>(maxLength: 5, nullable: true),
                    reference = table.Column<string>(maxLength: 200, nullable: false),
                    lat = table.Column<string>(maxLength: 20, nullable: true),
                    Lon = table.Column<string>(maxLength: 20, nullable: true),
                    type_address = table.Column<string>(maxLength: 5, nullable: true),
                    AgreementsId = table.Column<int>(nullable: false),
                    SuburbsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id_address);
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
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Agreement_TypeIntakeId",
                table: "Agreement",
                column: "TypeIntakeId");

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
                name: "IX_Agreement_Discount_id_agreement",
                table: "Agreement_Discount",
                column: "id_agreement");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Log_AgreementId",
                table: "Agreement_Log",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_Log_UserId",
                table: "Agreement_Log",
                column: "UserId");

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
                name: "IX_Consumption_MeterId",
                table: "Consumption",
                column: "MeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ClienteId",
                table: "Contact",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Control_ViewId",
                table: "Control",
                column: "ViewId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_AgreementId",
                table: "Debt",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_DebtPeriodId",
                table: "Debt",
                column: "DebtPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Detail_ServiceId",
                table: "Debt_Detail",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Period_TypePeriodId",
                table: "Debt_Period",
                column: "TypePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Derivative_AgreementId",
                table: "Derivative",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Folio_BranchOfficeId",
                table: "Folio",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Meter_AgreementId",
                table: "Meter",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ExternalOriginPaymentId",
                table: "Payment",
                column: "ExternalOriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OriginPaymentId",
                table: "Payment",
                column: "OriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PayMethodId",
                table: "Payment",
                column: "PayMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_State_CountriesId",
                table: "State",
                column: "CountriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_GroupStatusId",
                table: "Status",
                column: "GroupStatusId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_BranchOfficeId",
                table: "Terminal",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_User_TerminalId",
                table: "Terminal_User",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_User_UserId",
                table: "Terminal_User",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Town_StateId",
                table: "Town",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ExternalOriginPaymentId",
                table: "Transaction",
                column: "ExternalOriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_OriginPaymentId",
                table: "Transaction",
                column: "OriginPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PayMethodId",
                table: "Transaction",
                column: "PayMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TerminalUserId",
                table: "Transaction",
                column: "TerminalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TypeTransactionId",
                table: "Transaction",
                column: "TypeTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Detail_TransactionId",
                table: "Transaction_Detail",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Folio_TransactionId",
                table: "Transaction_Folio",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Type_GroupTypeId",
                table: "Type",
                column: "GroupTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_View_Profile_id_view",
                table: "View_Profile",
                column: "id_view");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Agreement_Discount");

            migrationBuilder.DropTable(
                name: "Agreement_Log");

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
                name: "Consumption");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Control");

            migrationBuilder.DropTable(
                name: "Debt_Detail");

            migrationBuilder.DropTable(
                name: "Derivative");

            migrationBuilder.DropTable(
                name: "Folio");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "System_Log");

            migrationBuilder.DropTable(
                name: "Tariff");

            migrationBuilder.DropTable(
                name: "Transaction_Detail");

            migrationBuilder.DropTable(
                name: "Transaction_Folio");

            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.DropTable(
                name: "View_Profile");

            migrationBuilder.DropTable(
                name: "Suburb");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Meter");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Debt");

            migrationBuilder.DropTable(
                name: "Group_Status");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Group_Type");

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
                name: "Debt_Period");

            migrationBuilder.DropTable(
                name: "External_Origin_Payment");

            migrationBuilder.DropTable(
                name: "Origin_Payment");

            migrationBuilder.DropTable(
                name: "Pay_Method");

            migrationBuilder.DropTable(
                name: "Terminal_User");

            migrationBuilder.DropTable(
                name: "Type_Transaction");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "Diameter");

            migrationBuilder.DropTable(
                name: "Type_Commertial_Business");

            migrationBuilder.DropTable(
                name: "Type_Consume");

            migrationBuilder.DropTable(
                name: "Type_Intake");

            migrationBuilder.DropTable(
                name: "Type_Regime");

            migrationBuilder.DropTable(
                name: "Type_Service");

            migrationBuilder.DropTable(
                name: "Type_State_Service");

            migrationBuilder.DropTable(
                name: "Type_Use");

            migrationBuilder.DropTable(
                name: "Type_Period");

            migrationBuilder.DropTable(
                name: "Terminal");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Branch_Office");
        }
    }
}
