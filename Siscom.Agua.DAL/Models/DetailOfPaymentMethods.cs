using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class DetailOfPaymentMethods
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_detail_payment_method")]
        public int Id { get; set; }
        [Column("card_number")]
        public string CardNumber { get; set; }
        [Column("authorization_bank")]
        public string AuthorizationBank { get; set; }
        [Column("check_issuance_series")]
        public string CheckIssuanceSeries { get; set; }
        [Column("account_number")]
        public string AccountNumber { get; set; }
        [Column("tracking_number")]
        public string TrackingNumber { get; set; }
        public string BankName { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
