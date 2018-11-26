using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Siscom.Agua.DAL.Models;

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

        /// <summary>
        /// Location
        /// </summary>
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Clasification> Clasifications { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Suburb> Suburbs { get; set; }
        public DbSet<Adress> Adresses { get; set; }

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
        public DbSet<TypeCommertialBusiness> TypeCommertialBusinesses { get; set; }
        public DbSet<TypeStateService> TypeStateServices { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<AgreementService> AgreementServices { get; set; }
        public DbSet<Diameter> Diameters { get; set; }
        public DbSet<AgreementDiscount> AgreementDiscounts { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Derivative> Derivatives { get; set; }

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

        /// <summary> 
        /// Groups
        /// </summary> 
        public DbSet<Type> Types { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<GroupStatus> GroupStatuses { get; set; }
        public DbSet<Status> Statuses { get; set; }


        /// <summary> 
        /// Calculation of debt
        /// </summary> 
        public DbSet<Debt> Debts { get; set; }
        public DbSet<TypePeriod> TypePeriods { get; set; }
        public DbSet<DebtPeriod> DebtPeriods { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }

        /// <summary> 
        /// Payment
        /// </summary> 
        public DbSet<OriginPayment> OriginPayments { get; set; }
        public DbSet<ExternalOriginPayment> ExternalOriginPayments { get; set; }
        public DbSet<Payment> Payments { get; set; }


        /// <summary>
        /// System
        /// </summary>
        public DbSet<SystemLog> SystemLogs { get; set; }

        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ViewProfile>().HasKey(sc => new { sc.ViewId, sc.RoleId });
            builder.Entity<AgreementService>().HasKey(x => new { x.IdService, x.IdAgreement });
            builder.Entity<AgreementDiscount>().HasKey(x => new { x.IdDiscount, x.IdAgreement });

            builder.Entity<ViewProfile>()
                .HasOne<View>(sc => sc.View)
                .WithMany(s => s.ViewProfiles)
                .HasForeignKey(sc => sc.ViewId);

            builder.Entity<ViewProfile>()
               .HasOne<ApplicationRol>(sc => sc.ApplicationRol)
               .WithMany(s => s.ViewProfiles)
               .HasForeignKey(sc => sc.RoleId);

            builder.Entity<AgreementService>()
                .HasOne<Service>(x => x.Service)
                .WithMany(y => y.AgreementServices)
                .HasForeignKey(x => x.IdService);

            builder.Entity<AgreementService>()
                .HasOne<Agreement>(x => x.Agreement)
                .WithMany(y => y.AgreementServices)
                .HasForeignKey(x => x.IdAgreement);

            builder.Entity<AgreementDiscount>()
               .HasOne<Agreement>(x => x.Agreement)
               .WithMany(y => y.AgreementDiscounts)
               .HasForeignKey(x => x.IdAgreement);

            builder.Entity<AgreementDiscount>()
              .HasOne<Discount>(x => x.Discount)
              .WithMany(y => y.AgreementDiscounts)
              .HasForeignKey(x => x.IdDiscount);

            builder.Entity<Agreement>()
               .Property(x => x.StratDate)
               .HasColumnType("date");

            builder.Entity<AgreementDiscount>()
                .Property(x => x.StartDate)
                .HasColumnType("date");

            builder.Entity<AgreementDiscount>()
                .Property(x => x.EndDate)
                .HasColumnType("date");

            /// <summary> 
            /// Cash Box 
            /// </summary> 
            builder.Entity<TerminalUser>()
                .Property(x => x.InOperation)
                .HasDefaultValue(false);

            builder.Entity<TerminalUser>()
                .Property(x => x.OpenDate)
                .HasColumnType("date");

            builder.Entity<Terminal>()
                .Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Entity<Folio>()
                .Property(x => x.DateCurrent)
                .HasDefaultValue(System.DateTime.Now);

            /// <summary> 
            /// Calculation of debt
            /// </summary> 
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

            builder.Entity<Debt>()
               .Property(x => x.FromDate)
               .HasColumnType("date");

            builder.Entity<Debt>()
               .Property(x => x.UntilDate)
               .HasColumnType("date");

            builder.Entity<Meter>()
               .Property(x => x.InstallDate)
               .HasColumnType("date");

            builder.Entity<Meter>()
               .Property(x => x.DeinstallDate)
               .HasColumnType("date");

            builder.Entity<Tariff>()
              .Property(x => x.FromDate)
              .HasColumnType("date");

            builder.Entity<Tariff>()
              .Property(x => x.UntilDate)
              .HasColumnType("date");

            base.OnModelCreating(builder);
        }
    }
}
