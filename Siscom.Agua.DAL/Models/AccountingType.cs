using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("accounting_type")]
    public class AccountingType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_accounting_type")]
        public int Id { get; set; }
        [Column("accounting_SAC")]
        public int AccountingSAC { get; set; }
        [Column("description"), StringLength(100)]
        public string Description { get; set; }
    }
}
