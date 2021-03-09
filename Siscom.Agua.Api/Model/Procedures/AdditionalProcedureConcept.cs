using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Procedures
{
    public class AdditionalProcedureConceptVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }


        public int AvailableProcedureId { get; set; }
        public AvailableProcedureVM AvailableProcedure { get; set; }
    }
}
