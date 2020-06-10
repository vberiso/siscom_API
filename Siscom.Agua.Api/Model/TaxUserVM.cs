using System;
using System.Collections.Generic;

namespace Siscom.Agua.Api.Model
{
    public class TaxUserVM
    {
        public TaxUserVM()
        {
            TaxAddresses = new List<TaxAddressVM>();
        }

        public int Id { get; set; }
        public String Name { get; set; }
        public String RFC { get; set; }
        public String CURP { get; set; }
        public String PhoneNumber { get; set; }
        public String EMail { get; set; }
        public bool IsActive { get; set; }
        public bool IsProvider { get; set; }
        public List<TaxAddressVM> TaxAddresses { get; set; }
    }
}
