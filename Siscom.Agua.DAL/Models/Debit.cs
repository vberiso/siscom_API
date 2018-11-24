using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debit")]
    public class Debit
    {
        [Key]
        [Column("id_debit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Column("debit_date"), Required]
        public DateTime DebitDate { get; set; }
        public DateTime Period { get; set; }
        public DateTime DebitFrom { get; set; }
        public DateTime DebitUntil { get; set; }

//TypeState*
//Discount
//TypeConsume
//TypeService
//TypeIntake
//Partial
//TypeReceipt(TypeDebit)*
    }
}
