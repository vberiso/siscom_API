using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt_Prepaid")]
    public class DebtPrepaid
    {
        [Key]
        [Column("id_debt_pepaid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, StringLength(5), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(150), Column("name_concept")]
        public string NameConcept { get; set; }
        [Column("original_amount"), Required]
        public decimal OriginalAmount { get; set; }
        [Column("payment_amount"), Required]
        public decimal PaymentAmount { get; set; }
        [Column("id_debt")]
        public int DebtId { get; set; }

        public int PrepaidDetailId { get; set; }
        public PrepaidDetail PrepaidDetail { get; set; }
    }
}
