using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class InfoOrderVM
    {
        public List<OrderWork> lstOrderWork { get; set; }
        public List<Agreement> lstAgreements { get; set; }
        public List<TaxUser> lstTaxUser { get; set; }
    }
}
