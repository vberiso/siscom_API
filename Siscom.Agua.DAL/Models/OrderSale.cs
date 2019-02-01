using System;
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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("date_order")]
        public DateTime DateOrder { get; set; }

        [Required,StringLength(30),Column("amount")]
        public string Amount { get; set; }

        [Required,StringLength(30),Column("on_account")]
        public string OnAccount { get; set; }

        [Required,StringLength(30),Column("year")]
        public string Year { get; set; }

        [Required,StringLength(30),Column("type")]
        public string Type { get; set; }

        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        [Required,Column("observation"),StringLength(100)]
        public string Observation { get; set; }


        [Column("id_origin")]
        public int IdOrigin { get; set; }

        [Column("id_tax_user")]
        public int IdTaxUser { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required,Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }

        [Required,Column( "period")]
        public string Period { get; set; }



        public int DivisionId { get; set; }
        public Division Division { get; set; }

        public ICollection<OrderSaleDetail> OrderSaleDetail { get; set; }




    }
}
