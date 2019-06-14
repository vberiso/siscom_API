using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("order_sale_status")]
    public class OrderSaleStatus
    {
        [Key]
        [Column("id_order_status"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_status"), Required]
        public string id_status { get; set; }
        [Column("order_status_date"), Required]
        public DateTime DebtStatusDate { get; set; }
        [Column("user"), StringLength(150), Required]
        public string User { get; set; }

        public int OrderSaleId { get; set; }
        public OrderSale OrderSale { get; set; }
    }
}
