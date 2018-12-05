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
        [Column("id_service")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }
        [Required, Column("order")]
        public Int16 Order { get; set; }
        [Required, Column("is_service")]
        public bool IsService { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Required, Column("in_agreement")]
        public bool InAgreement { get; set; }
        [Required, Column("is_active")]
        public bool IsActive { get; set; }

        public ICollection<TariffProduct> TariffProducts { get; set; }
    }
}
