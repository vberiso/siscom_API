using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Discount_Authorization_Detail")]
    public class DiscountAuthorizationDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_authorization_detail")]
        public int Id { get; set; }
        [Column("id_debt"), Required]
        public int DebtId { get; set; }
        [NotMapped]
        public Debt Debt { get; set; }
        [Column("id_order_sale"), Required]
        public int OrderSaleId { get; set; }
        [NotMapped]
        public OrderSale OrderSale { get; set; }

        public int DiscountAuthorizationId { get; set; }
        public DiscountAuthorization DiscountAuthorization { get; set; }
    }
}
