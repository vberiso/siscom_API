using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Agreement_Log")]
    public class AgreementLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_agreement_log")]
        public int Id { get; set; }
        [Column("agreement_log_date"), Required]
        public DateTime AgreementLogDate { get; set; }
        [Column("description"), StringLength(30), Required]
        public string Description { get; set; }
        [Column("observation")]
        public string Observation { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
    }
}
