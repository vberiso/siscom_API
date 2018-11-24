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
            Addresses = new HashSet<Adress>();
            AgreementServices = new HashSet<AgreementService>();
        }

        [Column("id_agreement"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("account"), StringLength(50), Required]
        public string Account { get; set; }
        [Column("account_date"), Required]
        public DateTime AccountDate { get; set; }
        [Column("derivatives"), Required]
        public int Derivatives { get; set; }
        [Column("start_date"), Required]
        public DateTime StratDate { get; set; }

        public TypeService TypeService { get; set; }
        public TypeUse TypeUse{ get; set; }
        public TypeConsume TypeConsume { get; set; }
        public TypeRegime TypeRegime { get; set; }
        public TypePeriod TypePeriod { get; set; }
        public TypeCommertialBusiness TypeCommertialBusiness { get; set; }
        public TypeStateService TypeStateService { get; set; }
        public TypeIntake TypeIntake { get; set; }
        public Diameter Diameter { get; set; }

        public ICollection<Client> Clients { get; set; }
        public ICollection<Adress> Addresses { get; set; }
        public ICollection<AgreementService> AgreementServices { get; set; }
        public ICollection<AgreementDiscount> AgreementDiscounts { get; set; }
    }
}
