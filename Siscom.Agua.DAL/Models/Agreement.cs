using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Agreement")]
    public class Agreement
    {
        public Agreement()
        {
            Clients = new HashSet<Client>();
            Addresses = new HashSet<Address>();
            Meters = new HashSet<Meter>();
            Debts = new HashSet<Debt>();
            Derivatives = new HashSet<Derivative>();
            AgreementServices = new HashSet<AgreementService>();
            Prepaids = new HashSet<Prepaid>();
            Notifications = new HashSet<Notification>();
            AgreementDetails = new HashSet<AgreementDetail>();
            AgreementFiles = new HashSet<AgreementFile>();
            AgreementRulerCalculations = new HashSet<AgreementRulerCalculation>();
            PartialPayments = new HashSet<PartialPayment>();
            AccountStatusInFiles = new HashSet<AccountStatusInFile>();
        }

        [Column("id_agreement"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("account"), StringLength(50)]
        public string Account { get; set; }
        [Column("account_date"), Required]
        public DateTime AccountDate { get; set; }
        [Column("num_derivatives"), Required]
        public int NumDerivatives { get; set; }
        [Column("start_date"), Required]
        public DateTime StratDate { get; set; }
        [Column("type_agreement"), StringLength(5), Required]
        public string TypeAgreement { get; set; }
        [Column("route"), StringLength(50), Required]
        public string Route { get; set; }
        [Column("token"), StringLength(11)]
        public string Token { get; set; }
        //[ForeignKey("TypeService")]
        public int TypeServiceId { get; set; }
        public TypeService TypeService { get; set; }

        //[ForeignKey("TypeUse")]
        public int TypeUseId { get; set; }
        public TypeUse TypeUse{ get; set; }

        //[ForeignKey("TypeConsume")]
        public int TypeConsumeId { get; set; }
        public TypeConsume TypeConsume { get; set; }

        //[ForeignKey("TypeRegime")]
        public int TypeRegimeId { get; set; }
        public TypeRegime TypeRegime { get; set; }

        //[ForeignKey("TypePeriod")]
        public int TypePeriodId { get; set; }
        public TypePeriod TypePeriod { get; set; }

        //[ForeignKey("TypeCommertialBusiness")]
        public int TypeCommertialBusinessId { get; set; }
        public TypeCommercialBusiness TypeCommertialBusiness { get; set; }

        //[ForeignKey("TypeStateService")]
        public int TypeStateServiceId { get; set; }
        public TypeStateService TypeStateService { get; set; }

        //[ForeignKey("TypeIntake")]
        public int TypeIntakeId { get; set; }
        public TypeIntake TypeIntake { get; set; }

        //[ForeignKey("Diameter")]
        public int DiameterId { get; set; }
        public Diameter Diameter { get; set; }

        public int TypeClassificationId { get; set; }
        public TypeClassification TypeClassification { get; set; }

        public ICollection<Client> Clients { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<AgreementService> AgreementServices { get; set; }
        public ICollection<AgreementDiscount> AgreementDiscounts { get; set; }
        public ICollection<Meter> Meters { get; set; }
        public ICollection<Debt> Debts { get; set; }
        public ICollection<Derivative> Derivatives { get; set; }
        public ICollection<AgreementLog> AgreementLogs { get; set; }
        public ICollection<Prepaid> Prepaids { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<AgreementDetail> AgreementDetails { get; set; }
        public ICollection<AgreementFile> AgreementFiles { get; set; }
        public ICollection<AgreementComment> AgreementComments { get; set; }
        public ICollection<AgreementRulerCalculation> AgreementRulerCalculations { get; set; }

        public ICollection<OrderWork> OrderWork { get; set; }
        public ICollection<PartialPayment> PartialPayments { get; set; }
        public ICollection<AccountStatusInFile> AccountStatusInFiles { get; set; }
    }
}
