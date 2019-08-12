using System;
using Siscom.Agua.Api.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementCommentVM
    {
       
        public int Id { get; set; }
       
        public DateTime DateIn { get; set; }
       
        public string Observation { get; set; }
        
        public bool IsVisible { get; set; }
        
        public string UserId { get; set; }
        
        public string UserName { get; set; }
        
        public DateTime DateOut { get; set; }

        public int AgreementId { get; set; }

        public AgreementVM Agreement { get; set; }
    }
}
