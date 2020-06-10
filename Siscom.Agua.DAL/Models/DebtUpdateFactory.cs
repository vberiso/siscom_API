using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("debt_update_factory")]
    public class DebtUpdateFactory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_debt_update_factor")]
        public int Id { get; set; }
        [Column("change_date")]
        public DateTime ChangeDate { get; set; }
        [Column("code_concept"), StringLength(5)]
        public string CodeConcept { get; set; }
        [Column("name_concept"), StringLength(5)]
        public string NameConcept { get; set; }
        [Column("original_amount")]
        public decimal OriginalAmount { get; set; }
        [Column("change_amount")]
        public decimal ChangeAmount { get; set; }
        public int DebtId { get; set; }
        public Debt Debt { get; set; }
    }
}
