using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Debt_Status")]
    public class DebtStatus
    {        
        [Key]
        [Column("id_debt_dtatus"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_status"), Required]
        public string id_status { get; set; }
        [Column("debt_dtatus_date"), Required]
        public DateTime DebtStatusDate { get; set; }
        [Column("user"), StringLength(150), Required]
        public string User { get; set; }

        public int DebtId { get; set; }
        public Debt Debt { get; set; }
    }
}
