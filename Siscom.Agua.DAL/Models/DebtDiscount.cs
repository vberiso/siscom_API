using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt_Discount")]
    public class DebtDiscount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_debt_discount")]
        public int Id { get; set; }
        [Required, StringLength(5), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(150), Column("name_concept")]
        public string NameConcept { get; set; }
        [Column("original_amount"), Required]
        public double OriginalAmount { get; set; }
        [Column("discount_amount"), Required]
        public double DiscountAmount { get; set; }

        public int DebtId { get; set; }
        public Debt Debt { get; set; }
    }
}
