using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Siscom.Agua.DAL.Models;
using Siscom.Agua.DAL.Models.ModelsProcedures;
using System;
using System.Linq;
using System.Text;

namespace Siscom.Agua.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRol, string>
    {
        readonly IConfiguration _configuration;

        /// <summary>
        /// Security
        /// </summary>
        public DbSet<Control> Controls { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<ViewProfile> ViewProfiles { get; set; }

        //public object Types(Func<object, bool> p)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Location
        /// </summary>
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Clasification> Clasifications { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Suburb> Suburbs { get; set; }
        public DbSet<Address> Adresses { get; set; }

        /// <summary>
        /// Agreement
        /// </summary>
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<TypeService> TypeServices { get; set; }
        public DbSet<TypeIntake> TypeIntakes { get; set; }
        public DbSet<TypeUse> TypeUses { get; set; }
        public DbSet<TypeConsume> TypeConsumes { get; set; }
        public DbSet<TypeRegime> TypeRegimes { get; set; }
        public DbSet<TypeCommercialBusiness> TypeCommertialBusinesses { get; set; }
        public DbSet<TypeStateService> TypeStateServices { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<AgreementService> AgreementServices { get; set; }
        public DbSet<Diameter> Diameters { get; set; }
        public DbSet<AgreementDiscount> AgreementDiscounts { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Derivative> Derivatives { get; set; }
        public DbSet<AgreementLog> AgreementLogs { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<FolioOrderSale> FolioOrderSales { get; set; }
        public DbSet<FolioOrderWork> FolioOrderWorks { get; set; }
        public DbSet<GenericFolios> GenericFolios { get; set; }
        public DbSet<TypeClassification> TypeClassifications { get; set; }
        public DbSet<Prepaid> Prepaids { get; set; }
        public DbSet<PrepaidDetail> PrepaidDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationDetail> NotificationDetails { get; set; }
        public DbSet<AgreementDetail> AgreementDetails { get; set; }
        public DbSet<AgreementFile> AgreementFiles { get; set; }
        public DbSet<AgreementComment> AgreementComments { get; set; }
        public DbSet<AgreementRulerCalculation> AgreementRulerCalculations { get; set; }
        public DbSet<AccountStatusInFile> AccountStatusInFiles { get; set; }
        public DbSet<PreAgreement> PreAgreements { get; set; }
        /// <summary>
        /// Accounting
        /// </summary>
        public DbSet<AccountingCode> AccountingCodes { get; set; }
        public DbSet<AccountingStatus> AccountingStatuses { get; set; }
        public DbSet<AccountingPayment> AccountingPayments { get; set; }
        public DbSet<AccountingType> AccountingTypes { get; set; }



        /// <summary> 
        /// Cash Box 
        /// </summary> 
        public DbSet<BranchOffice> BranchOffices { get; set; }
        public DbSet<Folio> Folios { get; set; }
        public DbSet<PayMethod> PayMethods { get; set; }
        public DbSet<Terminal> Terminal { get; set; }
        public DbSet<TerminalUser> TerminalUsers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TypeTransaction> TypeTransactions { get; set; }
        public DbSet<TransactionFolio> TransactionFolios { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }
        public DbSet<CancelAuthorization> CancelAuthorization { get; set; }
        public DbSet<TransactionCancellationRequest> TransactionCancellationRequests { get; set; }

        /// <summary> 
        /// Groups
        /// </summary> 
        public DbSet<Models.Type> Types { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<GroupStatus> GroupStatuses { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Catalogue> Catalogues { get; set; }
        public DbSet<GroupCatalogue> GroupCatalogues { get; set; }
        public DbSet<GroupTranslationCode> GroupTranslationCodes { get; set; }
        public DbSet<TranslationCode> TranslationCodes { get; set; }


        /// <summary> 
        /// Calculation of debt
        /// </summary> 
        public DbSet<Debt> Debts { get; set; }
        public DbSet<DebtAnnual> DebtAnnual { get; set; }
        public DbSet<DebtDetail> DebtDetails { get; set; }
        public DbSet<DebtStatus> DebtStatuses { get; set; }
        public DbSet<TypePeriod> TypePeriods { get; set; }
        public DbSet<DebtPeriod> DebtPeriods { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<TariffProduct> TariffProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DebtDiscount> DebtDiscounts { get; set; }
        public DbSet<DebtPrepaid> DebtPrepaids { get; set; }
        public DbSet<INPC> INPCs { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<DebtCalculation> DebtCalculations { get; set; }
        public DbSet<ProductParam> ProductParams { get; set; }
        public DbSet<ServiceParam> ServiceParams { get; set; }


        /// <summary> 
        /// Payment
        /// </summary> 
        public DbSet<OriginPayment> OriginPayments { get; set; }
        public DbSet<ExternalOriginPayment> ExternalOriginPayments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<TaxReceipt> TaxReceipts { get; set; }
        public DbSet<TaxReceiptCancel> TaxReceiptCancels { get; set; }
        public DbSet<DetailOfPaymentMethods> DetailOfPaymentMethods { get; set; }
        public DbSet<FilesTimbrado> FilesTimbrados { get; set; }

        /// <summary>
        /// Route
        /// </summary>
        public DbSet<Route> Routes { get; set; }

        /// <summary>
        /// System
        /// </summary>
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<SystemParameters> SystemParameters { get; set; }
        public DbSet<LogginLog> LogginLogs { get; set; }

        /// <summary>
        /// Notifications
        /// </summary>
        public DbSet<PushNotifications> PushNotifications { get; set; }

        /// <summary>
        /// NotificationFiles
        /// </summary>
        public DbSet<NotificationFiles> NotificationFiles { get; set; }

        /// <summary>
        /// Authorization
        /// </summary>
        public DbSet<Authorization> Authorizations { get; set; }

        /// <summary>
        /// Orders 
        /// </summary>
        public DbSet<TaxUser> TaxUsers { get; set; }
        public DbSet<TaxAddress> TaxAddresses { get; set; }
        public DbSet<OrderSale> OrderSales { get; set; }
        public DbSet<OrderSaleDetail> OrderSaleDetails { get; set; }
        public DbSet<Breach> Breaches { get; set; }
        public DbSet<BreachArticle> BreachArticles { get; set; }
        public DbSet<BreachList> BreachLists { get; set; }
        public DbSet<BreachDetail> BreachDetails { get; set; }
        public DbSet<AssignmentTicket> AssignmentTickets { get; set; }
        public DbSet<BreachWarranty> BreachWarranties { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<OrderSaleDiscount> OrderSaleDiscounts { get; set; }
        public DbSet<DiscountCampaign> DiscountCampaigns { get; set; }
        public DbSet<DiscountAuthorization> DiscountAuthorizations { get; set; }
        public DbSet<DiscountAuthorizationDetail> DiscountAuthorizationDetails { get; set; }
        public DbSet<TransitPolice> TransitPolices { get; set; }
        public DbSet<OrderSaleStatus> OrderSaleStatuses { get; set; }
        public DbSet<SkipArticles> SkipArticles { get; set; }

        /// <summary>
        /// Cancelations
        /// </summary>
        public DbSet<CancelProduct> CancelProduct { get; set; }

        /// <summary>
        /// BrandModel
        /// </summary>
        public DbSet<BrandModel> BrandModels { get; set; }
        /// <summary>
        /// VersionApp
        /// </summary>
        public DbSet<VersionApp> VersionApps { get; set; }

        /// <summary>
        /// OrderWork
        /// </summary>
        public DbSet<TechnicalRole> TechnicalRoles { get; set; }
        public DbSet<TechnicalTeam> TechnicalTeams { get; set; }
        public DbSet<TechnicalStaff> TechnicalStaffs { get; set; }
        public DbSet<OrderWork> OrderWorks { get; set; }
        public DbSet<OrderWorkStatus> OrderWorkStatus { get; set; }
        public DbSet<OrderParameters> OrderParameters { get; set; }
        public DbSet<OrderWorkDetail> OrderWorkDetails { get; set; }
        public DbSet<ReasonCatalog> ReasonCatalog { get; set; }
        public DbSet<LocationOrderWork> LocationOrderWorks { get; set; }
        public DbSet<OrderWorkReasonCatalog> OrderWorkReasonCatalog { get; set; }
        public DbSet<LocationOfAttentionOrderWork> LocationOfAttentionOrderWorks { get; set; }
        public DbSet<PhotosOrderWork> PhotosOrderWork { get; set; }
        public DbSet<FolioAccountStatement> FolioAccountStatements { get; set; }
        public DbSet<DispatchOrder> DispatchOrders { get; set; }
        public DbSet<MaterialMovements> MaterialMovements { get; set; }
        public DbSet<UnitMeasurement> UnitMeasurements { get; set; }
        public DbSet<MaterialList> MaterialLists { get; set; }
        public DbSet<GroupLists> GroupLists { get; set; }
        public DbSet<Lists> Lists { get; set; }
        public DbSet<OrderWorkList> OrderWorkLists { get; set; }
        public DbSet<OrderWorkListPictures> OrderWorkListPictures { get; set; }


        /// <summary>
        /// Otros
        /// </summary>
        /// 
        public DbSet<AccountsAccumulated> AccountsAccumulated { get; set; }
        public DbSet<InspectionFine> InspectionFines { get; set; }

        public DbSet<PostalMx> PostalMx { get; set; }

        /// <summary>
        /// Campañas
        /// </summary>
        public DbSet<CondonationCampaing> CondonationCampaings { get; set; }

        /// <summary>
        /// Convenios
        /// </summary>
        public DbSet<PartialPayment> PartialPayments { get; set; }
        public DbSet<PartialPaymentDetail> PartialPaymentDetails { get; set; }
        public DbSet<PartialPaymentDetailStatus> PartialPaymentDetailStatuses { get; set; }
        public DbSet<PartialPaymentDetailConcept> PartialPaymentDetailConcepts { get; set; }
        public DbSet<PartialPaymentDebt> PartialPaymentDebts { get; set; }

        /// <summary>
        /// PDFPaymentOnline
        /// </summary>
        /// 
        public DbSet<OnlinePaymentFile> OnlinePaymentFiles { get; set; }


        /// <summary>
        /// PagosAnuales
        /// </summary>
        /// 
        public DbSet<PagosAnuales> PagosAnuales { get; set; }


        /// <summary>
        /// Tabla temporal para condonacion anual.
        /// </summary>
        public DbSet<MovimientosDebt> MovimientosDebts { get; set; }

        /// <summary>
        /// Promociones
        /// </summary>
        public DbSet<PromotionGroup> PromotionGroup { get; set; }
        public DbSet<Promotions> Promotions { get; set; }
        public DbSet<PromotionDebt> PromotionDebt { get; set; }
        public DbSet<DebtCampaign> DebtCampaign { get; set; }
        public DbSet<DebtCampaignFiles> DebtCampaignFiles { get; set; }
        public DbSet<BenefitedCampaign> BenefitedCampaign { get; set; }

        public DbSet<ProofNoDebt> ProofNoDebts { get; set; }

        /// <summary>
        /// Ot Mobile
        /// </summary>
        public DbSet<Phones> Phones { get; set; }

        /// <summary>
        /// Valvula
        /// </summary>
        public DbSet<ValvulaControl> ValvulaControls { get; set; }
        public DbSet<ValveIncident> ValveIncidents { get; set; }
        public DbSet<ValveOperation> ValveOperations { get; set; }


        /// <summary>
        /// Tramites
        /// </summary>
        public DbSet<CitizenProcedure> CitizenProcedures { get; set; }
        public DbSet<OrderCitizenProcedure> OrderCitizenProcedures { get; set; }
        public DbSet<DateProcedure> DateProcedures { get; set; }
        public DbSet<DocumentProcedure> DocumentProcedures { get; set; }
        public DbSet<NoteProcedure> NoteProcedures { get; set; }
        public DbSet<AdditionalProcedureConcept> AdditionalProcedureConcepts { get; set; }
        public DbSet<AvailableProcedure> AvailableProcedures { get; set; }
        public DbSet<StepProcedure> StepProcedures { get; set; }
        public DbSet<RequirementForStep> RequirementForSteps { get; set; }



        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
            Database.SetCommandTimeout(180000);
        }

        //public override int SaveChanges()
        //{
        //    OnBeforeSaving();
        //    return base.SaveChanges();
        //}

        //public override int SaveChanges(bool acceptAllChangesOnSuccess)
        //{
            
        //    OnBeforeSaving();
        //    return base.SaveChanges(acceptAllChangesOnSuccess);
        //}
        protected override void OnModelCreating(ModelBuilder builder)
        {                    

            #region Address
            builder.Entity<Address>()
                   .HasOne<Agreement>(a => a.Agreements)
                   .WithMany(s => s.Addresses)
                   .HasForeignKey(s => s.AgreementsId);

            builder.Entity<Address>()
                   .HasOne<Suburb>(a => a.Suburbs)
                   .WithMany(s => s.Addresses)
                   .HasForeignKey(s => s.SuburbsId);
            #endregion

            #region AccountingCode
            builder.Entity<AccountingCode>()
               .Property(x => x.IsActive)
               .HasDefaultValue(true);
            #endregion

            #region AccountStatusInFile
            builder.Entity<AccountStatusInFile>()
                   .HasOne<Agreement>(sc => sc.Agreement)
                   .WithMany(s => s.AccountStatusInFiles)
                   .HasForeignKey(sc => sc.AgreementId);
            #endregion

            #region Agreement
            builder.Entity<Agreement>()
                   .HasOne<TypeService>(a => a.TypeService)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeServiceId);

            builder.Entity<Agreement>()
                   .HasOne<TypeUse>(a => a.TypeUse)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeUseId);

            builder.Entity<Agreement>()
                   .HasOne<TypeConsume>(a => a.TypeConsume)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeConsumeId);

            builder.Entity<Agreement>()
                   .HasOne<TypeRegime>(a => a.TypeRegime)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeRegimeId);

            builder.Entity<Agreement>()
                   .HasOne<TypePeriod>(a => a.TypePeriod)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypePeriodId);

            builder.Entity<Agreement>()
                   .HasOne<TypeCommercialBusiness>(a => a.TypeCommertialBusiness)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeCommertialBusinessId);

            builder.Entity<Agreement>()
                   .HasOne<TypeStateService>(a => a.TypeStateService)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeStateServiceId);

            builder.Entity<Agreement>()
                   .HasOne<TypeIntake>(a => a.TypeIntake)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.TypeIntakeId);

            builder.Entity<Agreement>()
                   .HasOne<Diameter>(a => a.Diameter)
                   .WithMany(s => s.Agreements)
                   .HasForeignKey(s => s.DiameterId);

            builder.Entity<Agreement>()
                  .HasOne<TypeClassification>(a => a.TypeClassification)
                  .WithMany(s => s.Agreements)
                  .HasForeignKey(s => s.TypeClassificationId);

            builder.Entity<Agreement>()
                  .HasMany(a => a.Debts)
                  .WithOne(s => s.Agreement);

            builder.Entity<Agreement>()
                 .HasMany(a => a.AgreementComments)
                 .WithOne(s => s.Agreement);

            builder.Entity<Agreement>()
                 .HasMany(a => a.AgreementRulerCalculations)
                 .WithOne(s => s.Agreement);
            
            builder.Entity<Agreement>()
                 .Property(x => x.StratDate)
                 .HasColumnType("date");

            builder.Entity<Agreement>()
                .Property(x => x.Route)
                .HasDefaultValue("0");
            #endregion

            #region AgreementComment
            builder.Entity<AgreementComment>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.AgreementComments)
                   .HasForeignKey(s => s.AgreementId);
            #endregion

            #region AgreementDetail
            builder.Entity<AgreementDetail>()
                  .HasOne<Agreement>(a => a.Agreement)
                  .WithMany(s => s.AgreementDetails)
                  .HasForeignKey(s => s.AgreementId);

            builder.Entity<AgreementDetail>()
                  .Property(x => x.Manifest)
                  .HasDefaultValue(false);

            #endregion

            #region AgreementDiscount
            builder.Entity<AgreementDiscount>().HasKey(x => new { x.IdDiscount, x.IdAgreement });

            builder.Entity<AgreementDiscount>()
                   .HasOne<Agreement>(x => x.Agreement)
                   .WithMany(y => y.AgreementDiscounts)
                   .HasForeignKey(x => x.IdAgreement);

            builder.Entity<AgreementDiscount>()
                   .HasOne<Discount>(x => x.Discount)
                   .WithMany(y => y.AgreementDiscounts)
                   .HasForeignKey(x => x.IdDiscount);

            builder.Entity<AgreementDiscount>()
                 .Property(x => x.StartDate)
                 .HasColumnType("date");

            builder.Entity<AgreementDiscount>()
                   .Property(x => x.EndDate)
                   .HasColumnType("date");
            #endregion

            #region AgreementFile
            builder.Entity<AgreementFile>()
                  .HasOne<Agreement>(a => a.Agreement)
                  .WithMany(s => s.AgreementFiles)
                  .HasForeignKey(s => s.AgreementId);

            builder.Entity<AgreementFile>()
                  .HasOne<ApplicationUser>(a => a.User)
                  .WithMany(s => s.AgreementFiles)
                  .HasForeignKey(s => s.UserId);

            builder.Entity<AgreementFile>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);

            builder.Entity<AgreementFile>()
                 .Property(x => x.Size)
                 .HasDefaultValue(0);

            #endregion

            #region AgreementLog
            builder.Entity<AgreementLog>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.AgreementLogs)
                   .HasForeignKey(s => s.AgreementId);

            builder.Entity<AgreementLog>()
                   .HasOne<ApplicationUser>(a => a.User)
                   .WithMany(s => s.AgreementLogs)
                   .HasForeignKey(s => s.UserId);
            #endregion

            #region AgreementRulerCalculation
            builder.Entity<AgreementRulerCalculation>()
               .HasOne<Agreement>(t => t.Agreement)
               .WithMany(r => r.AgreementRulerCalculations)
               .HasForeignKey(f => f.AgreementId);
            #endregion

            #region AgreementService
            builder.Entity<AgreementService>().HasKey(x => new { x.IdService, x.IdAgreement });

            builder.Entity<AgreementService>()
                   .HasOne<Service>(x => x.Service)
                   .WithMany(y => y.AgreementServices)
                   .HasForeignKey(x => x.IdService);

            builder.Entity<AgreementService>()
                   .HasOne<Agreement>(x => x.Agreement)
                   .WithMany(y => y.AgreementServices)
                   .HasForeignKey(x => x.IdAgreement);
            #endregion

            #region AssignmentTicket
            builder.Entity<AssignmentTicket>()
                    .HasOne<TransitPolice>(a => a.TransitPolice)
                    .WithMany(s => s.AssignmentTickets)
                    .HasForeignKey(s => s.TransitPoliceId);
            #endregion

            #region ApplicationUser
            builder.Entity<ApplicationUser>()
                .Property(x => x.IsActive)
                .HasDefaultValue(false);
            #endregion

            #region BenefitedCampaign
            builder.Entity<BenefitedCampaign>()
                 .Property(x => x.ApplicationDate)
                 .HasColumnType("date");

            builder.Entity<BenefitedCampaign>()
                  .Property(p => p.AmountDiscount)
                  .HasColumnType("decimal(18, 2)");
            #endregion

            #region BranchOffice
            builder.Entity<BranchOffice>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region Breach
            builder.Entity<Breach>()
                    .HasOne<ApplicationUser>(a => a.User)
                    .WithMany(s => s.Breaches)
                    .HasForeignKey(s => s.UserId);

            builder.Entity<Breach>()
                   .HasOne<TaxUser>(a => a.TaxUser)
                   .WithMany(s => s.Breaches)
                   .HasForeignKey(s => s.TaxUserId);
            #endregion

            #region BreachArticle
            builder.Entity<BreachArticle>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);
            #endregion

            #region BreachDetail           
            builder.Entity<BreachDetail>().HasKey(x => new { x.BreachId, x.BreachListId });

            builder.Entity<BreachDetail>()
                   .HasOne<Breach>(a => a.Breach)
                   .WithMany(s => s.BreachDetails)
                   .HasForeignKey(s => s.BreachId);

            builder.Entity<BreachDetail>()
                   .HasOne<BreachList>(a => a.BreachList)
                   .WithMany(s => s.BreachDetails)
                   .HasForeignKey(s => s.BreachListId);
            #endregion

            #region BreachList
            builder.Entity<BreachList>()
                   .HasOne<BreachArticle>(a => a.BreachArticle)
                   .WithMany(s => s.BreachLists)
                   .HasForeignKey(s => s.BreachArticleId);

            builder.Entity<BreachList>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);

            builder.Entity<BreachList>()
                 .Property(x => x.HaveBonification)
                 .HasDefaultValue(false);
            #endregion

            #region BreachWarranty
            builder.Entity<BreachWarranty>().HasKey(x => new { x.BreachId, x.WarrantyId }); 

            builder.Entity<BreachWarranty>()
                   .HasOne<Breach>(a => a.Breach)
                   .WithMany(s => s.BreachWarranties)
                   .HasForeignKey(s => s.BreachId);

            builder.Entity<BreachWarranty>()
                  .HasOne<Warranty>(a => a.Warranty)
                  .WithMany(s => s.BreachWarranty)
                  .HasForeignKey(s => s.WarrantyId);

            #endregion
           
            #region CancelAuthorization

            #endregion

            #region Catalogue
            builder.Entity<Catalogue>()
                   .HasOne<GroupCatalogue>(a => a.GroupCatalogue)
                   .WithMany(s => s.Catalogues)
                   .HasForeignKey(s => s.GroupCatalogueId);
            #endregion

            #region Client
            builder.Entity<Client>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Clients)

                   .HasForeignKey(s => s.AgreementId);
            builder.Entity<Client>()
                   .Property(x => x.TaxRegime)
                   .HasDefaultValue(false);
            #endregion

            #region Consumption
            builder.Entity<Consumption>()
                   .HasOne<Meter>(a => a.Meter)
                   .WithMany(s => s.Consumptions)
                   .HasForeignKey(s => s.MeterId);

            builder.Entity<Consumption>()
                   .Property(p => p.PreviousConsumption)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Consumption>()
                   .Property(p => p.CurrentConsumption)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Consumption>()
                   .Property(p => p.consumption)
                   .HasColumnType("decimal(18, 2)");

            #endregion

            #region Contact
            builder.Entity<Contact>()
                   .HasOne<Client>(a => a.Client)
                   .WithMany(s => s.Contacts)
                   .HasForeignKey(s => s.ClienteId);
            #endregion

            #region Debt
            builder.Entity<Debt>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Debts)
                   .HasForeignKey(s => s.AgreementId);

            builder.Entity<Debt>()
                   .Property(x => x.FromDate)
                   .HasColumnType("date");

            builder.Entity<Debt>()
                   .Property(x => x.UntilDate)
                   .HasColumnType("date");

            builder.Entity<Debt>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Debt>()
                   .Property(p => p.OnAccount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Debt>()
                  .Property(x => x.ExpirationDate)
                  .HasColumnType("date");

            builder.Entity<Debt>()
                  .Property(x => x.RechargesDate)
                  .HasColumnType("date");

            builder.Entity<Debt>()
                 .Property(x => x.RechargesDate)
                 .HasDefaultValue("1900-01-01");
            #endregion

            #region DebtAnnual
            builder.Entity<DebtAnnual>()
                  .Property(x => x.FromDate)
                  .HasColumnType("date");

            builder.Entity<DebtAnnual>()
                   .Property(x => x.UntilDate)
                   .HasColumnType("date");

            builder.Entity<DebtAnnual>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtAnnual>()
                   .Property(p => p.OriginalAmount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtAnnual>()
                   .Property(p => p.DiscountAmount)
                   .HasColumnType("decimal(18, 2)");
            #endregion

            #region DebtDetail
            builder.Entity<DebtDetail>()
                   .HasOne<Debt>(a => a.Debt)
                   .WithMany(s => s.DebtDetails)
                   .HasForeignKey(s => s.DebtId);

            builder.Entity<DebtDetail>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtDetail>()
                   .Property(p => p.OnAccount)
                   .HasColumnType("decimal(18, 2)");
     
            builder.Entity<DebtDetail>()
                  .Property(p => p.Quantity)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtDetail>()
                  .Property(x => x.Quantity)
                  .HasDefaultValue(1);

            builder.Entity<DebtDetail>()
               .Property(p => p.OldValue)
               .HasColumnType("decimal(18, 2)");

            #endregion

            #region DebtDiscount
            builder.Entity<DebtDiscount>()
                   .HasOne<Debt>(a => a.Debt)
                   .WithMany(s => s.DebtDiscounts)
                   .HasForeignKey(s => s.DebtId);

            builder.Entity<DebtDiscount>()
                   .Property(p => p.OriginalAmount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtDiscount>()
                   .Property(p => p.DiscountAmount)
                   .HasColumnType("decimal(18, 2)");
            #endregion

            #region DebtPeriod
            builder.Entity<DebtPeriod>()
               .Property(x => x.StartDate)
               .HasColumnType("date");

            builder.Entity<DebtPeriod>()
                .Property(x => x.EndDate)
                .HasColumnType("date");

            builder.Entity<DebtPeriod>()
                .Property(x => x.RunDate)
                .HasColumnType("date");

            builder.Entity<DebtPeriod>()
                .Property(x => x.RunHour)
                .HasColumnType("time");

            builder.Entity<DebtPeriod>()
                  .Property(x => x.ExpirationDate)
                  .HasColumnType("date");
            #endregion

            #region DebtPrepaid           
            builder.Entity<DebtPrepaid>()
                  .HasOne<PrepaidDetail>(a => a.PrepaidDetail)
                  .WithMany(s => s.DebtPrepaids)
                  .HasForeignKey(s => s.PrepaidDetailId);

            builder.Entity<DebtPrepaid>()
                  .Property(p => p.OriginalAmount)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtPrepaid>()
                  .Property(p => p.PaymentAmount)
                  .HasColumnType("decimal(18, 2)");
            #endregion

            #region DebtStatus
            builder.Entity<DebtStatus>()
                   .HasOne<Debt>(a => a.Debt)
                   .WithMany(s => s.DebtStatuses)
                   .HasForeignKey(s => s.DebtId);
            #endregion

            #region DebtUpdateFactory
            builder.Entity<DebtUpdateFactory>()
                   .HasOne<Debt>(a => a.Debt)
                   .WithMany(s => s.DebtUpdateFactories)
                   .HasForeignKey(s => s.DebtId);

            builder.Entity<DebtUpdateFactory>()
                   .Property(p => p.ChangeAmount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<DebtUpdateFactory>()
                  .Property(p => p.OriginalAmount)
                  .HasColumnType("decimal(18, 2)");

            #endregion

            #region Derivative
            builder.Entity<Derivative>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Derivatives)
                   .HasForeignKey(s => s.AgreementId);
            #endregion

            #region DetailOfPaymentMethods

            //builder.Entity<DetailOfPaymentMethods>()
            //      .HasOne<Payment>(a => a.Payment)
            //      .WithMany(s => s.DetailOfPaymentMethods)
            //      .HasForeignKey(s => s.ExternalOriginPaymentId);

            #endregion

            #region Diameter
            builder.Entity<Diameter>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region Discount
            builder.Entity<Discount>()
                  .Property(x => x.IsVariable)
                  .HasDefaultValue(false);
            #endregion

            #region DiscountAuthorization
            builder.Entity<DiscountAuthorization>()
                  .HasOne<ApplicationUser>(a => a.UserRequest)
                  .WithMany(s => s.DiscountAuthorizations)
                  .HasForeignKey(s => s.UserRequestId);

            builder.Entity<DiscountAuthorization>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<DiscountAuthorization>()
                  .Property(p => p.AmountDiscount)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<DiscountAuthorization>()
                 .Property(x => x.DiscountPercentage)
                 .HasDefaultValue(0);

            #endregion

            #region DiscountAuthorizationDetail
            builder.Entity<DiscountAuthorizationDetail>()
                 .HasOne<DiscountAuthorization>(a => a.DiscountAuthorization)
                 .WithMany(s => s.DiscountAuthorizationDetails)
                 .HasForeignKey(s => s.DiscountAuthorizationId);
            #endregion

            #region DiscountCampaign
            builder.Entity<DiscountCampaign>()
                  .Property(x => x.IsVariable)
                  .HasDefaultValue(false);

            builder.Entity<DiscountCampaign>()
                  .Property(x => x.EndDate)
                  .HasColumnType("date");

            builder.Entity<DiscountCampaign>()
                 .Property(x => x.StartDate)
                 .HasColumnType("date");
            #endregion

            #region Division
            builder.Entity<Division>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);
            #endregion

            #region ExternalOriginPayment
            builder.Entity<ExternalOriginPayment>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region Folio
            builder.Entity<Folio>()
                   .HasOne<BranchOffice>(a => a.BranchOffice)
                   .WithMany(s => s.Folios)
                   .HasForeignKey(s => s.BranchOfficeId);

            builder.Entity<Folio>()
               .Property(x => x.DateCurrent)
               .HasDefaultValue(System.DateTime.Now);

            #endregion

            #region FolioOrderSale           
            builder.Entity<FolioOrderSale>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);

            #endregion

            #region FolioOrderWork           
            builder.Entity<FolioOrderWork>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);
            #endregion

            #region FolioAccountStatement           
            builder.Entity<FolioAccountStatement>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);
            #endregion

            #region GenericFolios
            builder.Entity<GenericFolios>()
                .Property(x => x.IsActive)
                .HasDefaultValue(true);
            #endregion

            #region INPC
            builder.Entity<INPC>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Entity<INPC>()
                  .Property(p => p.Value)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<INPC>()
                  .Property(p => p.Surcharges)
                  .HasColumnType("decimal(18, 2)");

            #endregion

            #region InspectionFine
            builder.Entity<InspectionFine>()
                  .Property(x => x.IsActive)
                  .HasDefaultValue(true);

            builder.Entity<InspectionFine>()
                  .Property(p => p.Amount)
                  .HasColumnType("decimal(18, 2)");
            #endregion

            #region MaterialMovements

            builder.Entity<MaterialMovements>().HasKey(x => new { x.MaterialListId, x.OrderWorkId });

            builder.Entity<MaterialMovements>()
                .HasOne<MaterialList>(x => x.MaterialList)
                .WithMany(z => z.MaterialMovements)
                .HasForeignKey(x => x.MaterialListId);

            builder.Entity<MaterialMovements>()
                .HasOne<OrderWork>(x => x.OrderWork)
                .WithMany(z => z.MaterialMovements)
                .HasForeignKey(x => x.OrderWorkId);

            builder.Entity<MaterialMovements>()
                .Property(x => x.MovementDate)
                .HasColumnType("date");

            #endregion

            #region MaterialList

            builder.Entity<UnitMeasurement>()
                .HasOne<MaterialList>(x => x.MaterialList)
                .WithMany(m => m.UnitMeasurements)
                .HasForeignKey(x => x.MaterialListId);

            #endregion

            #region Meter
            builder.Entity<Meter>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Meters)
                   .HasForeignKey(s => s.AgreementId);

            builder.Entity<Meter>()
               .Property(x => x.InstallDate)
               .HasColumnType("date");

            builder.Entity<Meter>()
               .Property(x => x.DeinstallDate)
               .HasColumnType("date");
            #endregion

            #region Notification
            builder.Entity<Notification>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Notifications)
                   .HasForeignKey(s => s.AgreementId);

            builder.Entity<Notification>()
                  .Property(p => p.Subtotal)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<Notification>()
                 .Property(p => p.Tax)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<Notification>()
                 .Property(p => p.Rounding)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<Notification>()
                 .Property(p => p.Total)
                 .HasColumnType("decimal(18, 2)");
            #endregion

            #region NotificationDetail
            builder.Entity<NotificationDetail>()
                   .HasOne<Notification>(a => a.Notification)
                   .WithMany(s => s.NotificationDetails)
                   .HasForeignKey(s => s.NotificationId);
            #endregion

            #region NotificationFiles
            builder.Entity<NotificationFiles>()
                .Property(x => x.TypeFile)
                .HasDefaultValue("FI001");
            #endregion

            #region OrderSale
            builder.Entity<OrderSale>()
                   .HasOne<Division>(a => a.Division)
                   .WithMany(s => s.OrderSale)
                   .HasForeignKey(s => s.DivisionId);

            builder.Entity<OrderSale>()
                   .HasOne<TaxUser>(a => a.TaxUser)
                   .WithMany(s => s.OrderSales)
                   .HasForeignKey(s => s.TaxUserId);

            builder.Entity<OrderSale>()
                  .Property(x => x.ExpirationDate)
                  .HasColumnType("date");

            builder.Entity<OrderSale>()
                  .Property(p => p.Amount)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderSale>()
                   .Property(p => p.OnAccount)
                   .HasColumnType("decimal(18, 2)");

            #endregion

            #region  OrderSaleDetail
            builder.Entity<OrderSaleDetail>()
                   .HasOne<OrderSale>(a => a.OrderSale)
                   .WithMany(s => s.OrderSaleDetails)
                   .HasForeignKey(s => s.OrderSaleId);

            builder.Entity<OrderSaleDetail>()
                  .Property(p => p.Quantity)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderSaleDetail>()
                 .Property(p => p.Amount)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderSaleDetail>()
                   .Property(p => p.OnAccount)
                   .HasColumnType("decimal(18, 2)");

            #endregion

            #region  OrderSaleDiscount
            builder.Entity<OrderSaleDiscount>()
                  .HasOne<OrderSale>(a => a.OrderSale)
                  .WithMany(s => s.OrderSaleDiscounts)
                  .HasForeignKey(s => s.OrderSaleId);

            builder.Entity<OrderSaleDiscount>()
                   .Property(p => p.OriginalAmount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderSaleDiscount>()
                   .Property(p => p.DiscountAmount)
                   .HasColumnType("decimal(18, 2)");
            #endregion

            #region OrderSaleStatus
            builder.Entity<OrderSaleStatus>()
                 .HasOne<OrderSale>(a => a.OrderSale)
                 .WithMany(s => s.OrderSaleStatuses)
                 .HasForeignKey(s => s.OrderSaleId);

            #endregion

            #region OrderWork

            builder.Entity<OrderWork>()
                .Property(o => o.ObservationMobile)
                .HasDefaultValue("");

            builder.Entity<OrderWork>()
                .Property(o => o.ValvulaControlId)
                .HasDefaultValue(1);

            #endregion

            #region OrderWorkDetail

            builder.Entity<OrderWorkDetail>()
                .HasOne<OrderWork>(o => o.OrderWork)
                .WithMany(d => d.OrderWorkDetails)
                .HasForeignKey(k => k.OrderWorkId);

            #endregion

            #region OrderWorkStatus
            builder.Entity<OrderWorkStatus>()
               .HasOne<OrderWork>(a => a.OrderWork)
               .WithMany(s => s.OrderWorkStatus)
               .HasForeignKey(s => s.OrderWorkId);

            #endregion

            #region PhotosOrderWork
            
            builder.Entity<PhotosOrderWork>()
               .HasOne<OrderWork>(a => a.OrderWork)
               .WithMany(s => s.PhotosOrderWork)
               .HasForeignKey(s => s.OrderWorkId);

            #endregion

            #region OrderWorkList

            builder.Entity<OrderWorkList>()
                .HasOne<OrderWork>(o => o.OrderWork)
                .WithMany(d => d.OrderWorkLists)
                .HasForeignKey(k => k.OrderWorkId);

            #endregion

            #region OrderWorkListPictures

            builder.Entity<OrderWorkListPictures>()
                .HasOne<OrderWorkList>(ol => ol.OrderWorkList)
                .WithMany(p => p.OrderWorkListPictures)
                .HasForeignKey(k => k.OrderWorkListId);
              
            #endregion

            #region TechnicalStaff
            builder.Entity<TechnicalStaff>()
               .HasMany<OrderWork>(a => a.OrderWorks)
               .WithOne(s => s.TechnicalStaff)
               .HasForeignKey(s => s.TechnicalStaffId);

            builder.Entity<OrderWork>()
              .HasOne<Agreement>(a => a.Agreement)
              .WithMany(s => s.OrderWork)
              .HasForeignKey(b => b.AgrementId);
            //  .HasForeignKey(s => s.AgrementId);

            #endregion

            #region OrderWorkReasonCatalog
            builder.Entity<OrderWorkReasonCatalog>().HasKey(ORC => new { ORC.OrderWorkId, ORC.ReasonCatalogId });

            builder.Entity<OrderWorkReasonCatalog>()
                .HasOne(bc => bc.ReasonCatalog)
                .WithMany(b => b.OrderWorkReasonCatalogs)
                .HasForeignKey(bc => bc.ReasonCatalogId);

            builder.Entity<OrderWorkReasonCatalog>()
                .HasOne(bc => bc.OrderWork)
                .WithMany(c => c.OrderWorkReasonCatalogs)
                .HasForeignKey(bc => bc.OrderWorkId);
            #endregion

            #region LocationOrderWork
            
            builder.Entity<LocationOrderWork>().HasKey(ORC => new { ORC.OrderWorkId, ORC.LocationOfAttentionOrderWorkId });
            
            builder.Entity<LocationOrderWork>()
               .HasOne(bc => bc.LocationOfAttentionOrderWork)
               .WithMany(b => b.LocationOrderWorks)
               .HasForeignKey(bc => bc.LocationOfAttentionOrderWorkId);

            builder.Entity<LocationOrderWork>()
                .HasOne(bc => bc.OrderWork)
                .WithMany(c => c.LocationOrderWorks)
                .HasForeignKey(bc => bc.OrderWorkId);
            #endregion

            #region Lists
            builder.Entity<Lists>()
                .HasOne<GroupLists>(g => g.GroupLists)
                .WithMany(l => l.Lists)
                .HasForeignKey(x => x.GroupListsId);
            #endregion

            #region OriginPayment
            builder.Entity<OriginPayment>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region Payment
            builder.Entity<Payment>()
                   .HasOne<OriginPayment>(a => a.OriginPayment)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(s => s.OriginPaymentId);

            builder.Entity<Payment>()
                   .HasOne<ExternalOriginPayment>(a => a.ExternalOriginPayment)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(s => s.ExternalOriginPaymentId);

            builder.Entity<Payment>()
                   .HasOne<PayMethod>(a => a.PayMethod)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(s => s.PayMethodId);

            builder.Entity<Payment>()
                 .Property(p => p.Subtotal)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<Payment>()
                 .Property(p => p.Tax)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<Payment>()
                 .Property(p => p.Rounding)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<Payment>()
                .Property(p => p.Total)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Payment>()
                   .Property(x => x.HaveTaxReceipt)
                   .HasDefaultValue(false);

            builder.Entity<Payment>()
                   .Property(x => x.AgreementId )
                   .HasDefaultValue(0);

            builder.Entity<Payment>()
                   .Property(x => x.OrderSaleId)
                   .HasDefaultValue(0);
            #endregion

            #region PaymentDetail
            builder.Entity<PaymentDetail>()
                   .HasOne<Payment>(a => a.Payment)
                   .WithMany(s => s.PaymentDetails)
                   .HasForeignKey(s => s.PaymentId);

            builder.Entity<PaymentDetail>()
               .Property(p => p.Amount)
               .HasColumnType("decimal(18, 2)");

            builder.Entity<PaymentDetail>()
                   .Property(x => x.HaveTax)
                   .HasDefaultValue(false);

            builder.Entity<PaymentDetail>()
                   .Property(x => x.Tax)
                   .HasDefaultValue(0);
            #endregion

            #region PartialPayment
                builder.Entity<PartialPayment>()
                       .Property(p => p.Amount)
                       .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPayment>()
                       .Property(p => p.InitialPayment)
                       .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPayment>()
                       .Property(x => x.ExpirationDate)
                       .HasColumnType("date");

                builder.Entity<PartialPayment>()
                       .Property(x => x.FromDate)
                       .HasColumnType("date");

                builder.Entity<PartialPayment>()
                       .Property(x => x.UntilDate)
                       .HasColumnType("date");

                builder.Entity<PartialPayment>()
                       .HasOne<Agreement>(a => a.Agreement)
                       .WithMany(s => s.PartialPayments)
                       .HasForeignKey(s => s.AgreementId);

            #endregion

            #region PartialPaymentDetail
                builder.Entity<PartialPaymentDetail>()
                    .Property(p => p.Amount)
                    .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPaymentDetail>()
                       .Property(p => p.OnAccount)
                       .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPaymentDetail>()
                       .Property(p => p.Amount)
                       .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPaymentDetail>()
                      .Property(p => p.ReleasePeriod)
                      .HasColumnType("date");

            builder.Entity<PartialPaymentDetail>()
                       .HasOne<PartialPayment>(a => a.PartialPayment)
                       .WithMany(s => s.PartialPaymentDetails)
                       .HasForeignKey(s => s.PartialPaymentId);
            #endregion

            #region PartialPaymentDetailStatus
                builder.Entity<PartialPaymentDetailStatus>()
                       .HasOne<PartialPaymentDetail>(a => a.PartialPaymentDetail)
                       .WithMany(s => s.PartialPaymentDetailStatuses)
                       .HasForeignKey(s => s.PartialPaymentDetailId);

            #endregion

            #region PartialPaymentDetailConcept
                builder.Entity<PartialPaymentDetailConcept>()
                       .HasOne<PartialPaymentDetail>(a => a.PartialPaymentDetail)
                       .WithMany(s => s.PartialPaymentDetailConcepts)
                       .HasForeignKey(s => s.PartialPaymentDetailId);

                builder.Entity<PartialPaymentDetailConcept>()
                       .Property(p => p.Amount)
                       .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPaymentDetailConcept>()
                       .Property(p => p.OnAccount)
                       .HasColumnType("decimal(18, 2)");
            #endregion

            #region PartialPaymentDebt
                builder.Entity<PartialPaymentDebt>()
                       .HasOne<PartialPayment>(a => a.PartialPayment)
                       .WithMany(s => s.PartialPaymentDebts)
                       .HasForeignKey(s => s.PartialPaymentId);

                builder.Entity<PartialPaymentDebt>()
                           .Property(p => p.Amount)
                           .HasColumnType("decimal(18, 2)");

                builder.Entity<PartialPaymentDebt>()
                       .Property(p => p.OnAccount)
                       .HasColumnType("decimal(18, 2)");

            #endregion

            #region Product
            builder.Entity<Product>()
                   .HasOne<Division>(a => a.Division)
                   .WithMany(s => s.Products)
                   .HasForeignKey(s => s.DivisionId);

            builder.Entity<Product>()
                .Property(x => x.HaveAccount)
                .HasDefaultValue(false);
            #endregion

            #region Region  
            builder.Entity<PayMethod>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region PreAgreement
            builder.Entity<PreAgreement>()
                   .HasOne<TypeIntake>(a => a.TypeIntake)
                   .WithMany(s => s.PreAgreements)
                   .HasForeignKey(s => s.TypeIntakeId);
                       
            builder.Entity<PreAgreement>()
                   .HasOne<TypeService>(a => a.TypeService)
                   .WithMany(s => s.PreAgreements)
                   .HasForeignKey(s => s.TypeServiceId);
                                 
            builder.Entity<PreAgreement>()
                   .HasOne<TypeUse>(a => a.TypeUse)
                   .WithMany(s => s.PreAgreements)
                   .HasForeignKey(s => s.TypeUseId);

            builder.Entity<PreAgreement>()
                  .HasOne<TypeClassification>(a => a.TypeClassification)
                  .WithMany(s => s.PreAgreements)
                  .HasForeignKey(s => s.TypeClassificationId);  
            #endregion

            #region Prepaid
            builder.Entity<Prepaid>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Prepaids)
                   .HasForeignKey(s => s.AgreementId);

            builder.Entity<Prepaid>()
                 .Property(p => p.Amount)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<Prepaid>()
                 .Property(p => p.Accredited)
                 .HasColumnType("decimal(18, 2)");
            #endregion

            #region PrepaidDetail
            builder.Entity<PrepaidDetail>()
                   .HasOne<Prepaid>(a => a.Prepaid)
                   .WithMany(s => s.PrepaidDetails)
                   .HasForeignKey(s => s.PrepaidId);

            builder.Entity<PrepaidDetail>()
                 .Property(p => p.Amount)
                 .HasColumnType("decimal(18, 2)");
            #endregion

            #region Proof

            builder.Entity<ProofNoDebt>()
                .HasOne<Agreement>(a => a.Agreement);
                //.WithMany(x => x.ProofNoDebts)
                //.HasForeignKey(x => x.AgreementId);
            #endregion

            #region Region  
            builder.Entity<Region>()
                 .Property(p => p.Price)
                 .HasColumnType("decimal(18, 2)");
            #endregion

            #region Route
            builder.Entity<Route>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region State
            builder.Entity<State>()
                   .HasOne<Country>(a => a.Countries)
                   .WithMany(s => s.States)
                   .HasForeignKey(s => s.CountriesId);
            #endregion

            #region Status
            builder.Entity<Status>().HasKey(x => new { x.CodeName, x.GroupStatusId });
            #endregion

            #region Suburb
            builder.Entity<Suburb>()
                   .HasOne<Town>(a => a.Towns)
                   .WithMany(s => s.Suburbs)
                   .HasForeignKey(s => s.TownsId);

            builder.Entity<Suburb>()
                   .HasOne<Region>(a => a.Regions)
                   .WithMany(s => s.Suburbs)
                   .HasForeignKey(s => s.RegionsId);

            builder.Entity<Suburb>()
                   .HasOne<Clasification>(a => a.Clasifications)
                   .WithMany(s => s.Suburbs)
                   .HasForeignKey(s => s.ClasificationsId);

            builder.Entity<Suburb>()
                   .Property(x => x.RegistrationDate)
                   .HasColumnType("date");

            builder.Entity<Suburb>()
                   .Property(x => x.LastUpdateDate)
                   .HasColumnType("date");
            #endregion

            #region SystemParameters
            builder.Entity<SystemParameters>()
                 .Property(p => p.NumberColumn)
                 .HasColumnType("decimal(18, 2)");

            #endregion

            #region Tariff
            builder.Entity<Tariff>()
                   .HasOne<Service>(a => a.Service)
                   .WithMany(s => s.Tariffs)
                   .HasForeignKey(s => s.ServiceId);

            builder.Entity<Tariff>()
                   .HasOne<TypeIntake>(a => a.TypeIntake)
                   .WithMany(s => s.Tariffs)
                   .HasForeignKey(s => s.TypeIntakeId);

            builder.Entity<Tariff>()
                   .HasOne<TypeConsume>(a => a.TypeConsume)
                   .WithMany(s => s.Tariffs)
                   .HasForeignKey(s => s.TypeConsumeId);

            builder.Entity<Tariff>()
                   .Property(x => x.FromDate)
                   .HasColumnType("date");

            builder.Entity<Tariff>()
                   .Property(x => x.UntilDate)
                   .HasColumnType("date");

            builder.Entity<Tariff>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Tariff>()
                   .Property(p => p.StartConsume)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Tariff>()
                   .Property(p => p.EndConsume)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Tariff>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TariffParam
            builder.Entity<ServiceParam>()
                   .HasOne<Service>(a => a.Service)
                   .WithMany(s => s.ServiceParams)
                   .HasForeignKey(s => s.ServiceId);

            builder.Entity<ServiceParam>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TariffProduct
            builder.Entity<TariffProduct>()
                   .HasOne<Product>(a => a.Product)
                   .WithMany(s => s.TariffProducts)
                   .HasForeignKey(s => s.ProductId);

            builder.Entity<TariffProduct>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<TariffProduct>()
                  .Property(p => p.Percentage)
                  .HasColumnType("decimal(18, 2)");

            builder.Entity<TariffProduct>()
                  .Property(x => x.Type)
                  .HasDefaultValue("TTP01");
            #endregion

            #region TariffParam
            builder.Entity<ProductParam>()
                   .HasOne<Product>(a => a.Product)
                   .WithMany(s => s.ProductParams)
                   .HasForeignKey(s => s.ProductId);

            builder.Entity<ProductParam>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region  TaxAddress
            builder.Entity<TaxAddress>()
                   .HasOne<TaxUser>(a => a.TaxUser)
                   .WithMany(s => s.TaxAddresses)
                   .HasForeignKey(s => s.TaxUserId);

            #endregion

            #region TaxUser
            builder.Entity<TaxUser>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            
            builder.Entity<TaxUser>()
                  .Property(x => x.IsProvider)
                  .HasDefaultValue(false);
            #endregion

            #region  TaxReceipt
            builder.Entity<TaxReceipt>()
                   .HasOne<Payment>(a => a.Payment)
                   .WithMany(s => s.TaxReceipts)
                   .HasForeignKey(s => s.PaymentId);

            builder.Entity<TaxReceipt>()
                  .HasOne<ApplicationUser>(a => a.User)
                  .WithMany(s => s.TaxReceipts)
                  .HasForeignKey(s => s.UserId);
            #endregion

            #region TaxReceiptCancel
            builder.Entity<TaxReceiptCancel>()
                .HasOne<TaxReceipt>(t => t.TaxReceipt)
                .WithMany(r => r.TaxReceiptCancels)
                .HasForeignKey(f => f.TaxReceiptId);
            #endregion

            #region TechnicalRole
            builder.Entity<TechnicalRole>()
                  .Property(p => p.IsActive)
                  .HasDefaultValue(true);
            #endregion

            #region TechnicalStaff
            builder.Entity<TechnicalStaff>()
                .HasOne<TechnicalRole>(a => a.TechnicalRole)
                .WithMany(s => s.TechnicalStaffs)
                .HasForeignKey(s => s.TechnicalRoleId);

            builder.Entity<TechnicalStaff>()
               .HasOne<TechnicalTeam>(a => a.TechnicalTeam)
               .WithMany(s => s.TechnicalStaffs)
               .HasForeignKey(s => s.TechnicalTeamId);

            builder.Entity<TechnicalStaff>()
                  .Property(p => p.IsActive)
                  .HasDefaultValue(true);
            #endregion

            #region TechnicalTeam
            builder.Entity<TechnicalTeam>()
                 .Property(p => p.IsActive)
                 .HasDefaultValue(true);
            #endregion

            #region Terminal
            builder.Entity<Terminal>()
                   .HasOne<BranchOffice>(a => a.BranchOffice)
                   .WithMany(s => s.Terminals)
                   .HasForeignKey(s => s.BranchOfficeId);

            builder.Entity<Terminal>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Entity<Terminal>()
                   .Property(p => p.CashBox)
                   .HasColumnType("decimal(18, 2)");
            #endregion

            #region TerminalUser
            builder.Entity<TerminalUser>()
                   .HasOne<Terminal>(a => a.Terminal)
                   .WithMany(s => s.TerminalUsers)
                   .HasForeignKey(s => s.TerminalId);

            builder.Entity<TerminalUser>()
                   .HasOne<ApplicationUser>(a => a.User)
                   .WithMany(s => s.TerminalUsers)
                   .HasForeignKey(s => s.UserId);

            builder.Entity<TerminalUser>()
               .Property(x => x.InOperation)
               .HasDefaultValue(false);

            builder.Entity<TerminalUser>()
                .Property(x => x.OpenDate)
                .HasColumnType("date");
            #endregion

            #region Town
            builder.Entity<Town>()
                   .HasOne<State>(a => a.States)
                   .WithMany(s => s.Towns)
                   .HasForeignKey(s => s.StateId);
            #endregion            

            #region Transaction
            builder.Entity<Transaction>()
                   .HasOne<TerminalUser>(a => a.TerminalUser)
                   .WithMany(s => s.Transactions)
                   .HasForeignKey(s => s.TerminalUserId);

            builder.Entity<Transaction>()
                   .HasOne<TypeTransaction>(a => a.TypeTransaction)
                   .WithMany(s => s.Transactions)
                   .HasForeignKey(s => s.TypeTransactionId);

            //builder.Entity<Transaction>()
            //       .HasOne<PayMethod>(a => a.PayMethod)
            //       .WithMany(s => s.Transactions)
            //       .HasForeignKey(s => s.PayMethodId);

            builder.Entity<Transaction>()
                   .HasOne<OriginPayment>(a => a.OriginPayment)
                   .WithMany(s => s.Transactions)
                   .HasForeignKey(s => s.OriginPaymentId);

            builder.Entity<Transaction>()
                   .HasOne<ExternalOriginPayment>(a => a.ExternalOriginPayment)
                   .WithMany(s => s.Transactions)
                   .HasForeignKey(s => s.ExternalOriginPaymentId);

            builder.Entity<Transaction>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Transaction>()
                   .Property(p => p.Tax)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Transaction>()
                   .Property(p => p.Rounding)
                   .HasColumnType("decimal(18, 2)");

            builder.Entity<Transaction>()
                   .Property(p => p.Total)
                   .HasColumnType("decimal(18, 2)");
            #endregion

            #region TransactionDetail
            builder.Entity<TransactionDetail>()
                   .HasOne<Transaction>(a => a.Transaction)
                   .WithMany(s => s.TransactionDetails)
                   .HasForeignKey(s => s.TransactionId);

            builder.Entity<TransactionDetail>()
                   .Property(p => p.Amount)
                   .HasColumnType("decimal(18, 2)");
            #endregion

            #region TransactionFolio
            builder.Entity<TransactionFolio>()
                   .HasOne<Transaction>(a => a.Transaction)
                   .WithMany(s => s.TransactionFolios)
                   .HasForeignKey(s => s.TransactionId);
            #endregion

            #region TransitPolice
            builder.Entity<TransitPolice>()
                  .HasOne<ApplicationUser>(a => a.User)
                  .WithMany(s => s.TransitPolices)
                  .HasForeignKey(s => s.UserId);

            builder.Entity<TransitPolice>()
             .Property(x => x.IsActive)
             .HasDefaultValue(true);
            #endregion            

            #region TranslationCode
            builder.Entity<TranslationCode>().HasKey(x => new { x.ProductId, x.GroupTranslationCodeId, x.Type });
            #endregion

            #region Type
            //builder.Entity<Models.Type>()
            //       .HasOne<GroupType>(a => a.GroupType)
            //       .WithMany(s => s.Types)
            //       .HasForeignKey(s => s.GroupTypeId);
            builder.Entity<Models.Type>().HasKey(x => new { x.CodeName, x.GroupTypeId });
            #endregion

            #region TypeClassification
            builder.Entity<TypeClassification>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeCommercialBusiness
            builder.Entity<TypeCommercialBusiness>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeConsume
            builder.Entity<TypeConsume>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeIntake
            builder.Entity<TypeIntake>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypePeriod
            builder.Entity<TypePeriod>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeRegime
            builder.Entity<TypeRegime>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeService
            builder.Entity<TypeService>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeStateService
            builder.Entity<TypeStateService>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeTransaction
            builder.Entity<TypeTransaction>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region TypeUse
            builder.Entity<TypeUse>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
            #endregion

            #region Valve
            builder.Entity<ValveIncident>()
                   .HasOne<ValvulaControl>(a => a.ValvulaControl)
                   .WithMany(s => s.ValveIncidents)
                   .HasForeignKey(s => s.ValvulaControlId);

            builder.Entity<ValveOperation>()
                   .HasOne<ValvulaControl>(a => a.ValvulaControl)
                   .WithMany(s => s.ValveOperations)
                   .HasForeignKey(s => s.ValvulaControlId);

            var valvulaInicial = new ValvulaControl()
            {
                Id = 1,
                Description = "Sin Valvula",
                Reference = "Sin referencia",
                Latitude = "19.0954579",
                Longitude = "-98.2792209",
                Type = "OT011",
                IsActive = true,
                ActualState = "",
                Diameter = "",
                HydraulicCircuit = "",
                LastServiceDate = DateTime.Now,
                PhysicalState = ""
            };
            builder.Entity<ValvulaControl>().HasData(new ValvulaControl[] { valvulaInicial });

            builder.Entity<OrderWork>()
                   .HasOne<ValvulaControl>(a => a.ValvulaControl)
                   .WithMany(s => s.OrderWorks)
                   .HasForeignKey(s => s.ValvulaControlId);

            #endregion

            #region Version
            builder.Entity<VersionApp>()
                   .Property(x => x.PublishDate)
                   .HasColumnType("date");
            #endregion

            #region ViewProfile
            builder.Entity<ViewProfile>().HasKey(sc => new { sc.ViewId, sc.RoleId });

            builder.Entity<ViewProfile>()
                   .HasOne<View>(sc => sc.View)
                   .WithMany(s => s.ViewProfiles)
                   .HasForeignKey(sc => sc.ViewId);
           
            builder.Entity<ViewProfile>()
                   .HasOne<ApplicationRol>(sc => sc.ApplicationRol)
                   .WithMany(s => s.ViewProfiles)
                   .HasForeignKey(sc => sc.RoleId);
            
            builder.Entity<ViewProfile>().HasKey(sc => new { sc.ViewId, sc.RoleId });

            builder.Entity<ViewProfile>()
                   .HasOne<View>(sc => sc.View)
                   .WithMany(s => s.ViewProfiles)
                   .HasForeignKey(sc => sc.ViewId);

            builder.Entity<ViewProfile>()
                   .HasOne<ApplicationRol>(sc => sc.ApplicationRol)
                   .WithMany(s => s.ViewProfiles)
                   .HasForeignKey(sc => sc.RoleId);
            #endregion

            #region DebtCampaign

            builder.Entity<DebtCampaign>()
                 .Property(x => x.Status)
                 .HasDefaultValue("ECD01");

            //builder.Entity<DebtCampaignFiles>()
            //     .Property(x => x.IsInvitation)
            //     .HasDefaultValue(true);
      



        builder.Entity<DebtCampaign>()
                .HasOne<Agreement>(sc => sc.Agreement)
                .WithMany(s => s.DebtCampaign)
                .HasForeignKey(b => b.AgreementId); 
            #endregion

            #region otros
            builder.Entity<ReasonCatalog>()
              .Property(x => x.IsActive)
              .HasDefaultValue(true);
            #endregion

            #region TRAMITES

            #region Date Procedure
            builder.Entity<DateProcedure>()
                   .HasOne<CitizenProcedure>(c => c.CitizenProcedure)
                   .WithMany(s => s.DateProcedures)
                   .HasForeignKey(s => s.CitizenProcedureId);

            builder.Entity<DateProcedure>()
                .Property(x => x.Done)
                .HasDefaultValue(false);

            builder.Entity<DateProcedure>()
                .Property(x => x.CreateDate)
                .HasDefaultValue("getdate()");
            #endregion

            #region Order Citizen Procedure
            builder.Entity<OrderCitizenProcedure>()
                .HasKey(t => new { t.OrderSaleId, t.CitizenProcedureId });

            builder.Entity<OrderCitizenProcedure>()
                  .HasOne<OrderSale>(c => c.OrderSale)
                  .WithMany(s => s.OrderCitizenProcedures)
                  .HasForeignKey(s => s.OrderSaleId);

            builder.Entity<OrderCitizenProcedure>()
                .HasOne<CitizenProcedure>(c => c.CitizenProcedure)
                .WithMany(s => s.OrderCitizenProcedures)
                .HasForeignKey(s => s.CitizenProcedureId);

            #endregion

            #region Document Procedure
            builder.Entity<DocumentProcedure>()
                  .HasOne<CitizenProcedure>(c => c.CitizenProcedure)
                  .WithMany(s => s.DocumentProcedures)
                  .HasForeignKey(s => s.CitizenProcedureId);

            builder.Entity<DocumentProcedure>()
                .Property(x => x.UploadDate)
                .HasDefaultValue("getdate()");

            builder.Entity<DocumentProcedure>()
               .Property(x => x.Sha512)
               .HasColumnType("text");
            #endregion

            #region Note Procedure
            builder.Entity<NoteProcedure>()
                 .HasOne<CitizenProcedure>(c => c.CitizenProcedure)
                 .WithMany(s => s.NoteProcedures)
                 .HasForeignKey(s => s.CitizenProcedureId);

            builder.Entity<NoteProcedure>()
               .Property(x => x.CreateDate)
               .HasDefaultValue("getdate()");
            #endregion

            #region Requirements For Step
            builder.Entity<RequirementForStep>()
                .HasOne<StepProcedure>(s => s.StepProcedure)
                .WithMany(s => s.RequirementForSteps)
                .HasForeignKey(s => s.StepProcedureId);
            #endregion

            #region Citizen Procedure
            builder.Entity<CitizenProcedure>()
                .HasOne<AvailableProcedure>(d => d.AvailableProcedure); //Revisar para continuar

            builder.Entity<CitizenProcedure>()
                .Property(x => x.ClosingRemark)
                .HasColumnType("text");

            builder.Entity<CitizenProcedure>()
                .Property(x => x.BeginDate)
                .HasDefaultValue("getdate()");

            builder.Entity<CitizenProcedure>()
                .Property(x => x.CurrentStep)
                .HasDefaultValue(1);

            builder.Entity<CitizenProcedure>()
                .Property(x => x.MeetAllRequirements)
                .HasDefaultValue(false);
            #endregion

            #region Aditional Procedure Concepts
            builder.Entity<AdditionalProcedureConcept>()
                .HasOne<AvailableProcedure>(s => s.AvailableProcedure)
                .WithMany(s => s.AdditionalProcedureConcepts)
                .HasForeignKey(s => s.AvailableProcedureId);

            builder.Entity<AdditionalProcedureConcept>()
                  .Property(p => p.Cost)
                  .HasColumnType("decimal(18, 2)");
            #endregion

            #region Available Procedure
            builder.Entity<AvailableProcedure>()
                .HasOne<Division>(s => s.Division)
                .WithMany(s => s.AvailableProcedures)
                .HasForeignKey(s => s.DivisionId);

            builder.Entity<AvailableProcedure>()
                 .Property(p => p.Cost)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<AvailableProcedure>()
               .Property(x => x.IsActive)
               .HasDefaultValue(true);
            #endregion

            #region Step Procedure
            builder.Entity<StepProcedure>()
               .HasOne<AvailableProcedure>(s => s.AvailableProcedure)
               .WithMany(s => s.StepProcedures)
               .HasForeignKey(s => s.AvailableProcedureId);

            builder.Entity<StepProcedure>()
               .Property(x => x.CanDate)
               .HasDefaultValue(false);

            builder.Entity<StepProcedure>()
               .Property(x => x.CanDocument)
               .HasDefaultValue(false);

            builder.Entity<StepProcedure>()
               .Property(x => x.CanNote)
               .HasDefaultValue(false);

            builder.Entity<StepProcedure>()
               .Property(x => x.CanOrder)
               .HasDefaultValue(false);

            #endregion

            #endregion


            base.OnModelCreating(builder);
        }

        private void OnBeforeSaving()
        {
            var changes = from e in this.ChangeTracker.Entries()
                          where e.State != EntityState.Unchanged
                          select e;
            var now = DateTime.UtcNow.ToLocalTime();
            StringBuilder data = new StringBuilder();
            foreach (var change in changes)
            {
                var entityName = change.Entity.GetType().Name;

                if (change.State == EntityState.Added)
                {

                }
                else if (change.State == EntityState.Modified)
                {
                    //var item = change.Cast<IEntity>().Entity;
                    //var item = change.
                    //var originalValues = this.Entry(i)
                    //foreach (var o in change.Properties)
                    //{
                    //    //var properties = change.Property(o);
                    //}
                    var primaryKey = GetPrimaryKeyValue(change).ToString();
                    foreach (var prop in change.OriginalValues.Properties)
                    {
                        var originalValue = change.OriginalValues[prop].ToString();
                        var currentValue = change.CurrentValues[prop].ToString();
                        if(originalValue != currentValue)
                        {
                            data.Append("[PropertyName] = " + prop + " >")
                                .Append("[OldValue] = " + originalValue + " >")
                                .Append("[NewValue] = " + currentValue);
                            
                        }
                    }
                }
            }

        }

        private object GetPrimaryKeyValue(EntityEntry change)
        {
            var objectStateEntry = change.Metadata.FindPrimaryKey().Properties.Select(p => change.Property(p.Name));
            return objectStateEntry.Single();
            //return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }

        //object GetPrimaryKeyValue(DbEntityEntry entry)
        //{
        //    var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
        //    return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        //}
    }
}
