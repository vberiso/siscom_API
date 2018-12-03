using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Transaction")]
    public class Transaction
    {
        public Transaction()
        {
            TransactionFolios = new HashSet<TransactionFolio>();
            TransactionDetails = new HashSet<TransactionDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_transaction")]
        public int Id { get; set; }
        [Column("folio"), StringLength(40)]
        public string Folio { get; set; }
        [Column("date_transaction"), Required]
        public DateTime DateTransaction { get; set; }
        [Column("sign"), Required]
        public bool Sign { get; set; }
        [Column("amount"), Required]
        public double Amount { get; set; }
        [Column("tax"), Required]
        public double Tax { get; set; }
        [Column("rounding"), Required]
        public double Rounding { get; set; }        
        [Column("aplication"), StringLength(20), Required]
        public string Aplication { get; set; }
        [Column("cancellation_folio"), StringLength(40)]
        public string CancellationFolio { get; set; }        
        [Column("authorization_origin_payment"), StringLength(50)]
        public string AuthorizationOriginPayment { get; set; }

        [ForeignKey("TerminalUser")]
        public int TerminalUserId { get; set; }
        public TerminalUser TerminalUser { get; set; }

        public TypeTransaction TypeTransaction { get; set; }

        [ForeignKey("PayMethod")]
        public int PayMethodId { get; set; }
        public PayMethod PayMethod { get; set; }

        public OriginPayment OriginPayment { get; set; }
        public ExternalOriginPayment ExternalOriginPayment { get; set; }

        public ICollection<TransactionFolio> TransactionFolios { get; set; }
        public ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}
