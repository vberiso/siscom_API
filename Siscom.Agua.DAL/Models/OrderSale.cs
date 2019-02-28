﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Order_Sale")]
    public class OrderSale
    {
        public OrderSale()
        {
            OrderSaleDetail = new HashSet<OrderSaleDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_sale")]
        public int Id { get; set; }
        [Required, StringLength(30), Column("folio")]
        public string Folio { get; set; }
        [Required, Column("date_order")]
        public DateTime DateOrder { get; set; }
        [Required, Column("amount")]
        public decimal Amount { get; set; }
        [Required,Column("on_account")]
        public decimal OnAccount { get; set; }
        [Required,Column("year")]
        public Int16 Year { get; set; }
        [Required, Column("period")]
        public Int16 Period { get; set; }
        [Required,StringLength(5),Column("type")]
        public string Type { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("observation")]
        public string Observation { get; set; }
        [Column("id_origin")]
        public int IdOrigin { get; set; }
        [Column("id_tax_user")]
        public int IdTaxUser { get; set; }
        [NotMapped]
        public TaxUser TaxUser { get; set; }
        [Required,Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }        

        public int DivisionId { get; set; }
        public Division Division { get; set; }

        public ICollection<OrderSaleDetail> OrderSaleDetail { get; set; }
    }
}
