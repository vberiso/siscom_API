using System;
using System.Collections.Generic;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class BillingYear
    {
        public string conde_concept { get; set; }
        public string name_concept { get; set; }
        public decimal amount { get; set; }
        public bool have_tax { get; set; }
        public int id_discount { get; set; }
        public string discount { get; set; }
    }
}
