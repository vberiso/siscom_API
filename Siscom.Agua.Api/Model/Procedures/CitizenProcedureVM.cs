using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Procedures
{
    public class CitizenProcedureVM
    {

        public int Id { get; set; }
        public string Folio { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string ClosingRemark { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int CurrentStep { get; set; }
        public string Status { get; set; }
        public bool MeetAllRequirements { get; set; }


        public int AvailableProcedureId { get; set; }
        public AvailableProcedureVM AvailableProcedure { get; set; }

        public ICollection<DateProcedureVM> DateProcedures { get; set; }
        public ICollection<DocumentProcedureVM> DocumentProcedures { get; set; }
        public ICollection<NoteProcedureVM> NoteProcedures { get; set; }
        public ICollection<OrderCitizenProcedureVM> OrderCitizenProcedures { get; set; }
    }
}
