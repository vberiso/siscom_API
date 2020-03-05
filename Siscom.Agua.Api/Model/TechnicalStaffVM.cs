using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Model
{
    public class TechnicalStaffVM : DAL.Models.TechnicalStaff
    {
        //public int Id { get; set; }        
        //public string Name { get; set; }       
        //public string Phone { get; set; }        
        //public bool IsActive { get; set; }        
        //public int TechnicalRoleId { get; set; }
        //public TechnicalRole TechnicalRole { get; set; }        
        //public int TechnicalTeamId { get; set; }
        //public TechnicalTeam TechnicalTeam { get; set; }
        //public ICollection<OrderWork> OrderWorks { get; set; }

        public string Nick { get; set; }
        public string IMEI { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int DivisionId { get; set; }
    }
}
