using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class DebtVM
    {
    
   
        public int Id { get; set; }
      
        public DateTime DebitDate { get; set; }
       
        public DateTime FromDate { get; set; }
        
        public DateTime UntilDate { get; set; }
     
        public int Derivatives { get; set; }
      
        public string TypeIntake { get; set; }
    
        public string TypeService { get; set; }
       
        public string Consumption { get; set; }
       
        public string Discount { get; set; }
       
        public decimal Amount { get; set; }
        
        public decimal OnAccount { get; set; }

        public Int16 Year { get; set; }
       
        public string Type { get; set; }
    
        public string DescriptionType { get; set; }
       
        public string Status { get; set; }
        
        public string DescriptionStatus { get; set; }
        
        public string NewStatus { get; set; }
     
        public int? DebtPeriodId { get; set; }
       
        public DateTime ExpirationDate { get; set; }
        
        public DateTime RechargesDate { get; set; }
       
        public string observations { get; set; }

        public int AgreementId { get; set; }
        public AgreementVM Agreement { get; set; }

        public ICollection<DebtDetailVM> DebtDetails { get; set; }
        //public ICollection<DebtStatus> DebtStatuses { get; set; }
        //public ICollection<DebtDiscount> DebtDiscounts { get; set; }

    }
}
