using System;
using System.Collections.Generic;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class OrderWorkReasonCatalog
    {

        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }

        public int ReasonCatalogId { get; set; }
        public ReasonCatalog ReasonCatalog { get; set; }

    }
}
