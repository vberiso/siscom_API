using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Payment_Detail")]
    public class PaymentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_transaction_detail")]
        public int Id { get; set; }
        [Column("code_concept"), StringLength(10)]
        public string CodeConcept { get; set; }
        [Column("description"), StringLength(150)]
        public string Description { get; set; }
        [Column("amount"), Required]
        public double amount { get; set; }
        [Column("id_debt"), Required]
        public int DebtId { get; set; }
        [Column("id_prepaid"), Required]
        public int PrepaidId { get; set; }

        
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
