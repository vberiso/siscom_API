using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        [Column("id_discount"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        [Column("percentage"), Required]
        public Int16 Percentage { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        [Column("month"), Required]
        public Int16 Month { get; set; }
        [Column("start_date")]
        public DateTime? StartDate { get; set; }
        [Column("end_date")]
        public DateTime? EndDate { get; set; }
        [Required, Column("in_agreement")]
        public bool InAgreement { get; set; }
        [Column("is_variable")]
        public bool IsVariable { get; set; }

        public ICollection<AgreementDiscount> AgreementDiscounts { get; set; }
    }
}
