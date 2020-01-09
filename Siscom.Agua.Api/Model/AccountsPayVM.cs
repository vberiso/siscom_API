using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AccountsPayVM
    {
        public string id_agreement { get; set; }
        public int from_date { get; set; }
        public int until_date { get; set; }
        public DateTime payment_date { get; set; }
    }
}
