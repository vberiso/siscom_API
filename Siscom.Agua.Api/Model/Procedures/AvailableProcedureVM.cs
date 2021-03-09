using System;
using Siscom.Agua.Api.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Procedures
{
    public class AvailableProcedureVM
    {
        public int Id { get; set; }
        public string ProcedureDescription { get; set; }
        public decimal Cost { get; set; }
        public bool IsActive { get; set; }


        public int DivisionId { get; set; }
        public Division Division { get; set; }

        public ICollection<AdditionalProcedureConceptVM> AdditionalProcedureConcepts { get; set; }
        public ICollection<CitizenProcedureVM> CitizenProcedures { get; set; }
        public ICollection<StepProcedureVM> StepProcedures { get; set; }
    }
}
