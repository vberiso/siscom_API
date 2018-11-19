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
        public DbSet<TypePeriod> TypePeriods { get; set; }        
        public DbSet<TypeCommertialBusiness> TypeCommertialBusinesses { get; set; }
        public DbSet<TypeStateService> TypeStateServices { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<AgreementService> AgreementServices { get; set; }
        public DbSet<Diameter> Diameters { get; set; }

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

        /// <summary> 
        /// Types 
        /// </summary> 
        public DbSet<Type> Types { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }

        public ApplicationDbContext()
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

            builder.Entity<Folio>()
                .HasIndex(x => x.Initial)
                .IsUnique();

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

            base.OnModelCreating(builder);
        }
    }
}
