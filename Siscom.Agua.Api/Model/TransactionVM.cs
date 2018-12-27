using System;
using System.Collections.Generic;
using Siscom.Agua.DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransactionVM
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public bool Sign { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public string PercentageTax { get; set; }
        public decimal Rounding { get; set; }
        public decimal Total { get; set; }
        public string Aplication { get; set; }
        public int TypeTransactionId { get; set; }
        public int PayMethodId { get; set; }
        public int TerminalUserId { get; set; }
        public string Cancellation { get; set; }
        public string AuthorizationOriginPayment { get; set; }
        public int OriginPaymentId { get; set; }
        public int ExternalOriginPaymentId { get; set; }
        public string Type { get; set; }
        public string PaytStatus { get; set; }
        public int AgreementId { get; set; }
        public Int16 Percentage { get; set; }
        public List<TransactionDetail> transactionDetails { get; set; }
    }
}
