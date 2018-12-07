using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{   
    [Table("Product")]
    public class Product
    {
        public Product()
        {
            TariffProducts = new HashSet<TariffProduct>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_product")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }
        [Required, Column("order")]
        public Int16 Order { get; set; }
        [Column("parent")]
        public int Parent { get; set; }
        [Required, Column("have_tariff")]
        public bool HaveTariff { get; set; }
        [Required, Column("is_active")]
        public bool IsActive { get; set; }

        public ICollection<TariffProduct> TariffProducts { get; set; }
    }
}
