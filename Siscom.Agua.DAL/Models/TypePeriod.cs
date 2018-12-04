using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Type_Period")]
    public class TypePeriod
    {
        public TypePeriod()
        {
            Agreements = new HashSet<Agreement>();
            Discounts = new HashSet<Discount>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_type_period")]
        public int Id { get; set; }
        [Required, StringLength(15), Column("name")]
        public string Name { get; set; }
        [Column("mounth"), Required]
        public Int16 Mounth { get; set; }

        public ICollection<Agreement> Agreements { get; set; }
        public ICollection<Discount> Discounts { get; set; }
    }
}
