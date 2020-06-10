using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Model
{
    public class TechnicalStaffVM : DAL.Models.TechnicalStaff
    {
        public string Nick { get; set; }
        public string IMEI { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int DivisionId { get; set; }
        public Division Division { get; set; }
    }
}
