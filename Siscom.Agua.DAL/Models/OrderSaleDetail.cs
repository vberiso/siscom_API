using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Order_Sale_Detail")]
    public class OrderSaleDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_sale_detail")]
        public int Id { get; set; }
        [Required, Column("quantity")]
        public decimal Quantity { get; set; }
        [Required, Column("unity"), StringLength(18)]
        public string Unity { get; set; }
        [Required, Column("unit_price")]
        public decimal UnitPrice { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Required, Column("description")]
        public string Description { get; set; }
        [Required, StringLength(10), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(500), Column("name_concept")]
        public string NameConcept { get; set; }
        [Required, Column("amount")]
        public decimal Amount { get; set; }
        [Required, Column("on_account")]
        public decimal OnAccount { get; set; }
        [NotMapped]
        public decimal OnPayment { get; set; }
        //[NotMapped]
        [Column("tax"), Required]
        public decimal Tax { get; set; }

        public int OrderSaleId { get; set; }
        public OrderSale OrderSale { get; set; }
    }
}
