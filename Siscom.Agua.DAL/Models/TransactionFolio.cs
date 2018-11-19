using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Transaction_Folio")]
    public class TransactionFolio
    {
        [Key]
        [Column("folio"), Required]
        public string Folio { get; set; }
        public DateTime DatePrint { get; set; }

        public Transaction Transaction { get; set; }
    }
}
