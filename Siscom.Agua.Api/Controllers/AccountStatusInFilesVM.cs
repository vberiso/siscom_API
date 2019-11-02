using Siscom.Agua.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers
{
    public class AccountStatusInFilesVM
    {
        
        public int Id { get; set; }
       
        public string UserId { get; set; }
       
        public string UserName { get; set; }
       
        public DateTime GenerationDate { get; set; }
    
        public string FileName { get; set; }
        
        public string Folio { get; set; }
        
        public int AgreementId { get; set; }
        public AgreementVM Agreement { get; set; }
    }
}
