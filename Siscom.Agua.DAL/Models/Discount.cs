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
        [Column("id_discount"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        [Column("percentpercentage"), Required]
        public Int16 Percentpercentage { get; set; }
        [Column("is_active"), Required]
        public bool is_active { get; set; }

        public TypePeriod Validity { get; set; }
        public ICollection<AgreementDiscount> AgreementDiscounts { get; set; }
    }
}
