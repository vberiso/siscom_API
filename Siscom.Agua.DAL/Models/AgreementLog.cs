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
        [Column("description"), StringLength(80), Required]
        public string Description { get; set; }
        [Column("observation")]
        public string Observation { get; set; }
        [Column("visible"), Required]
        public bool Visible { get; set; }
        [Column("old_value"), Required]
        public string OldValue { get; set; }
        [Column("new_value"), Required]
        public string NewValue { get; set; }
        [Column("action"), Required]
        public string Action { get; set; }
        [Column("controller"), Required]
        public string Controller { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
    }
}
