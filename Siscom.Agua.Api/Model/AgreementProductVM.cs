using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementProductVM
    {
        public int Id { get; set; }
        public List<AdressVM> Adresses { get; set; }
        public List<ClientVM> Clients { get; set; }
    }
}
