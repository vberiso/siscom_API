using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("accounting_code")]
    public class AccountingCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_accounting_code")]
        public int Id { get; set; }
        [Column("origin")]
        public char Origin { get; set; }
        [Column("code_concept")]
        public int CodeConcept { get; set; }
        [Column("name_concept"),StringLength(400)]
        public string NameConcept { get; set; }
        [Column("id_divition")]
        public Int16 IdDivition { get; set; }
        [Column("name_divition"), StringLength(200)]
        public string NameDivition { get; set; }
        [Column("code_sac")]
        public int CodeSac { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
