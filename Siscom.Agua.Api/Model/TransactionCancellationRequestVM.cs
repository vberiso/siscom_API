using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionCancellationRequestVM
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime DateRequest { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public string  BranchOffice { get; set; }
        public decimal Amount { get; set; }
        public string PrintingFolio { get; set; }
        public string Manager { get; set; }
        public DateTime DateAuthorization { get; set; }        
        public string ManagerObservation { get; set; }        
        public string KeyFirebase { get; set; }
        public int TransactionId { get; set; }        
    }
}
