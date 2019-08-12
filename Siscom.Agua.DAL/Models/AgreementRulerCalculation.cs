using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("agreement_ruler_calculation")]
    public class AgreementRulerCalculation
    {
        [Key]
        [Column("id_agreement_ruler"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("serviceId")]
        public int ServiceId { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("date_in")]
        public DateTime DateIN { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
    }
}
