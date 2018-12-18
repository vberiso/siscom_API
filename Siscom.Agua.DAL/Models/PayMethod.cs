using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Pay_Method")]
    public class PayMethod
    {
        public PayMethod()
        {
            Transactions = new HashSet<Transaction>();
            Payments = new HashSet<Payment>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_pay_method")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
