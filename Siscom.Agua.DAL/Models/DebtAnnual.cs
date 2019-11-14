using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("debt_annual")]
    public class DebtAnnual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_debt_annual")]
        public int Id { get; set; }
        [Column("debit_date")]
        public DateTime DebitDate { get; set; }
        [Column("from_date")]
        public DateTime FromDate { get; set; }
        [Column("until_date")]
        public DateTime UntilDate { get; set; }
        [Column("type_intake"), StringLength(50)]
        public string TypeIntake { get; set; }
        [Column("type_service")]
        public string TypeService { get; set; }
        [Column("year")]
        public Int16 Year { get; set; }
        [Column("type"), StringLength(5)]
        public string Type { get; set; }
        [Column("status"), StringLength(5)]
        public string Status { get; set; }
        [Column("sequential")]
        public int Sequential { get; set; }
        [Column("code_concept"), StringLength(5)]
        public string CodeConcept { get; set; }
        [Column("name_concept"), StringLength(500)]
        public string NameConcept { get; set; }
        [Column("Amount")]
        public decimal Amount { get; set; }
        [Column("have_tax")]
        public bool HaveTax { get; set; }
        [Column("debt_id")]
        public int DebtId { get; set; }
        [Column("agreement_id")]
        public int AgreementId { get; set; }
    }
}
