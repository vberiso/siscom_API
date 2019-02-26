using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Siscom.Agua.DAL.Models;
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
        public DbSet<TypeClassification> TypeClassifications { get; set; }
        public DbSet<Prepaid> Prepaids { get; set; }
        public DbSet<PrepaidDetail> PrepaidDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationDetail> NotificationDetails { get; set; }
        public DbSet<AgreementDetail> AgreementDetails { get; set; }
        public DbSet<AgreementFile> AgreementFiles { get; set; }


        


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

        /// <summary> 
        /// Groups
        /// </summary> 
        public DbSet<Models.Type> Types { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<GroupStatus> GroupStatuses { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Catalogue> Catalogues { get; set; }
        public DbSet<GroupCatalogue> GroupCatalogues { get; set; }


        /// <summary> 
        /// Calculation of debt
        /// </summary> 
        public DbSet<Debt> Debts { get; set; }
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

        /// <summary>
        /// System
        /// </summary>
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<SystemParameters> SystemParameters { get; set; }

        /// <summary>
        /// Notifications
        /// </summary>
        public DbSet<PushNotifications> PushNotifications { get; set; }

        /// <summary>
        /// Authorization
        /// </summary>
        public DbSet<Authorization> Authorizations { get; set; }

        /// <summary>
        /// Orders
        /// </summary>
        public DbSet<TaxUser>  TaxUsers { get; set; }
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
                 .Property(x => x.StratDate)
                 .HasColumnType("date");
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
                    .HasOne<ApplicationUser>(a => a.User)
                    .WithMany(s => s.AssignmentTickets)
                    .HasForeignKey(s => s.UserId);
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

            #region Derivative
            builder.Entity<Derivative>()
                   .HasOne<Agreement>(a => a.Agreement)
                   .WithMany(s => s.Derivatives)
                   .HasForeignKey(s => s.AgreementId);
            #endregion

            #region Diameter
            builder.Entity<Diameter>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
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

            #region INPC
            builder.Entity<INPC>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Entity<INPC>()
                  .Property(p => p.Value)
                  .HasColumnType("decimal(18, 2)");
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

            #region OrderSale
            builder.Entity<OrderSale>()
                   .HasOne<Division>(a => a.Division)
                   .WithMany(s => s.OrderSale)
                   .HasForeignKey(s => s.DivisionId);

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
                   .WithMany(s => s.OrderSaleDetail)
                   .HasForeignKey(s => s.OrderSaleId);

            builder.Entity<OrderSaleDetail>()
                 .Property(p => p.Amount)
                 .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderSaleDetail>()
                   .Property(p => p.OnAccount)
                   .HasColumnType("decimal(18, 2)");

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
            #endregion

            #region PaymentDetail
            builder.Entity<PaymentDetail>()
                   .HasOne<Payment>(a => a.Payment)
                   .WithMany(s => s.PaymentDetails)
                   .HasForeignKey(s => s.PaymentId);

            builder.Entity<PaymentDetail>()
               .Property(p => p.Amount)
               .HasColumnType("decimal(18, 2)");
            #endregion

            #region Product
            builder.Entity<Product>()
                   .HasOne<Division>(a => a.Division)
                   .WithMany(s => s.Products)
                   .HasForeignKey(s => s.DivisionId);
            #endregion

            #region Region  
            builder.Entity<PayMethod>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);
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

            #region Region  
            builder.Entity<Region>()
                 .Property(p => p.Price)
                 .HasColumnType("decimal(18, 2)");
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

            builder.Entity<Payment>()
                   .Property(x => x.HaveTaxReceipt)
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

            #region Type
            builder.Entity<Models.Type>()
                   .HasOne<GroupType>(a => a.GroupType)
                   .WithMany(s => s.Types)
                   .HasForeignKey(s => s.GroupTypeId);
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
