using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementVM
    {
        public AgreementVM()
        {
            Adresses = new List<AdressVM>();
            Clients = new List<ClientVM>();
            ServicesId = new List<int>();
            AgreementDetails = new List<AgreementDetailVM>();
            AccountStatusInFiles = new List<AccountStatusInFileVM>();
        }

        public int Id { get; set; }
        public string Account { get; set; }
        [StringLength(50), Required]
        public string Route { get; set; }
        [Required]
        public int Derivatives { get; set; }
        [Required]
        public int TypeServiceId { get; set; }
        [Required]
        public int TypeUseId { get; set; }
        [Required]
        public int TypeConsumeId { get; set; }
        [Required]
        public int TypeRegimeId { get; set; }
        [Required]
        public int TypePeriodId { get; set; }
        [Required]
        public int TypeCommertialBusinessId { get; set; }
        [Required]
        public int TypeStateServiceId { get; set; }
        [Required]
        public int TypeIntakeId { get; set; }
        [Required]
        public int TypeClasificationId { get; set; }
        [Required]
        public int DiameterId { get; set; }
        [Required]
        public string TypeAgreement { get; set; }
        [Required]
        public int AgreementPrincipalId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Observations { get; set; }

        public List<int> ServicesId { get; set; }
        public List<AdressVM> Adresses { get; set; }
        public List<ClientVM> Clients { get; set; }
        public List<AgreementDetailVM> AgreementDetails { get; set; }

        public OrderWork OrderWork { get; set; }

        public ICollection<AccountStatusInFileVM> AccountStatusInFiles { get; set; }
        public ICollection<DebtVM> Debts { get; set; }
    }
}
