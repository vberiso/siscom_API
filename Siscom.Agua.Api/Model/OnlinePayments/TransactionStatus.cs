using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.OnlinePayments
{
    public class TransactionStatus
    {
        public TransactionStatus()
        {
            TransactionStatusDetails = new HashSet<TransactionStatusDetail>();
        }
        public int IdTransaction { get; set; }
        public string Folio { get; set; }
        public DateTime DateTransaction { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column( TypeName = "decimal(18, 2)")]
        public decimal Tax { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rounding { get; set; }
        [Column( TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }
        public string Aplication { get; set; }
        public string NumberAuthorizationPayment { get; set; }
        public string NameBank { get; set; }
        public string Reference { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Account { get; set; }
        public int AgreementId { get; set; }
        public bool HaveTaxReceipt { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Commission { get; set; }

        public ICollection<TransactionStatusDetail> TransactionStatusDetails { get; set; }
    }
}
