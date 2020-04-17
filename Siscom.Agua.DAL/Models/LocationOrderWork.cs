using System;
using System.Collections.Generic;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class LocationOrderWork
    {
        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }
        public int LocationOfAttentionOrderWorkId { get; set; }
        public LocationOfAttentionOrderWork LocationOfAttentionOrderWork { get; set; }
    }
}
