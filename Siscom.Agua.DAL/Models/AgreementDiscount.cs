using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Agreement_Discount")]
    public class AgreementDiscount
    {
        [Column("id_discount"), Required]
        public int IdDiscount { get; set; }
        public Discount Discount { get; set; }

        [Column("id_agreement"), Required]
        public int IdAgreement { get; set; }
        public Agreement Agreement { get; set; }

        [Column("start_date"), Required]
        public DateTime StartDate { get; set; }
        [Column("end_date"), Required]
        public DateTime EndDate { get; set; }

        [Column("is_active"), Required]
        public bool IsActive { get; set; }
    }
}
