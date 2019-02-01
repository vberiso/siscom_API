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
        public int    Id            { get; set; }

        [Required,Column("quantity"),StringLength(30)]
        public string Quantity      { get; set; }

        [Required,Column("unity"),StringLength(30)]
        public string Unity         { get; set; }

        [Required,Column("description"),StringLength(100)]
        public string Description   { get; set; }

        [Required,Column("code"),StringLength(30)]
        public string CodeConcept   { get; set; }

        [Required,Column("name"),StringLength(200)]
        public string NameConcept   { get; set; }

        [Required,Column("unit"),StringLength(30)]
        public string UnitPrice     { get; set; }

        [Required,Column("amount"),StringLength(30)]
        public string Amount        { get; set; }

        [Required,Column("on_account"),StringLength(30)]
        public string OnAccount     { get; set; }

        [Required,Column("haveTax"),StringLength(30)]
        public string HaveTax       { get; set; }


        public int OrderSaleId { get; set; }
        public OrderSale OrderSale { get; set; }


    }
}
