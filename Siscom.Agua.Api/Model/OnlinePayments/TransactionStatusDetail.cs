using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.OnlinePayments
{
    public class TransactionStatusDetail
    {
        public int Id { get; set; }
        public string CodeConcept { get; set; }
        public string AccountNumber { get; set; }
        public string UnitMeasurement { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int DebtId { get; set; }
        public int OrderSaleId { get; set; }
        public bool HaveTax { get; set; }
        public decimal Tax { get; set; }
        public string Type { get; set; }
        public int TransactionStatusId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }
}
