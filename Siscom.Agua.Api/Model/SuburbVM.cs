using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class SuburbVM
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int TownId { get; set; }
        public int RegionId { get; set; }
        public int ClasificationId { get; set; }
    }
}
