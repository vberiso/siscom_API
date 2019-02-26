using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Product_Param")]
    public class ProductParam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_product_param")]
        public int Id { get; set; }
        [Required, StringLength(20), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(500), Column("name_concept")]
        public string NameConcept { get; set; }
        [Column("unit_measurement"), StringLength(10), Required]
        public string UnitMeasurement { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
