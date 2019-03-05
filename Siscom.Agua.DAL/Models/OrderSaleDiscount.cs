using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Order_Sale_Discount")]
    public class OrderSaleDiscount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_sale_discount")]
        public int Id { get; set; }
        [Required, StringLength(10), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(500), Column("name_concept")]
        public string NameConcept { get; set; }
        [Column("original_amount"), Required]
        public decimal OriginalAmount { get; set; }
        [Column("discount_amount"), Required]
        public decimal DiscountAmount { get; set; }
        [Column("discount_percentage"), Required]
        public Int16 DiscountPercentage { get; set; }

        public int OrderSaleId { get; set; }
        public OrderSale OrderSale { get; set; }
    }
}
