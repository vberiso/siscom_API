using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("External_Origin_Payment")]
    public class ExternalOriginPayment
    {
        public ExternalOriginPayment()
        {
            Payments = new HashSet<Payment>();
            Transactions = new HashSet<Transaction>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_external_origin_payment")]
        public int Id { get; set; }
        [Required, StringLength(15), Column("name")]
        public string Name { get; set; }
        [Required, Column("is_bank")]
        public bool IsBank { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
