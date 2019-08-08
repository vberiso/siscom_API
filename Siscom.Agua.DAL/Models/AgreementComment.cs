using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("agreement_comment")]
    public class AgreementComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_agreement_comment")]
        public int Id { get; set; }
        [Column("date_in")]
        public DateTime DateIn { get; set; }
        [Column("observation"), StringLength(800)]
        public string Observation { get; set; }
        [Column("is_visible")]
        public bool IsVisible { get; set; }
        [Column("user_id"), StringLength(50)]
        public string UserId { get; set; }
        [Column("user_name"), StringLength(50)]
        public string UserName { get; set; }
        [Column("date_out")]
        public DateTime DateOut { get; set; }
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
    }
}
