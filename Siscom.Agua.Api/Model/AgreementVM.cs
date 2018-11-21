using System;
using System.Collections.Generic;
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
        }

        public int Id { get; set; }
        public string Account { get; set; }
        //public DateTime AccountDate { get; set; }
        public int Derivatives { get; set; }

        public int TypeServiceId { get; set; }
        public int TypeUseId { get; set; }
        public int TypeConsumeId { get; set; }
        public int TypeRegimeId { get; set; }
        public int TypePeriodId { get; set; }
        public int TypeCommertialBusinessId { get; set; }
        public int TypeStateServiceId { get; set; }
        public int TypeIntakeId { get; set; }
        public int DiameterId { get; set; }

        public List<AdressVM> Adresses { get; set; }
        public List<ClientVM> Clients { get; set; }
    }
}
