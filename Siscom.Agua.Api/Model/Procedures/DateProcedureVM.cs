using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Procedures
{
    public class DateProcedureVM
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime FinishDate { get; set; }

        public String Reason { get; set; }

        public String Place { get; set; }

        public Boolean Done { get; set; }

        public String UserId { get; set; }


        
        public int DivisionId { get; set; }


        public int CitizenProcedureId { get; set; }
        public CitizenProcedureVM CitizenProcedure { get; set; }
    }
}
