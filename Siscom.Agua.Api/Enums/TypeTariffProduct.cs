using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Enums
{
    public class TypeTariffProduct
    {
        public enum By
        {
            Factor = 1,
            Percentage = 2,
            Variable = 3,
            Amount = 4,
        }
    }
}
