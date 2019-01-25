using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ProductVM
    {
        public AgreementProductVM Agreement { get; set; }
        public Debt Debt { get; set; }
    }
}
