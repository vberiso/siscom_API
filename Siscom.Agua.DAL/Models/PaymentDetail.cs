using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Payment_Detail")]
    public class PaymentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_transaction_detail")]
        public int Id { get; set; }
        [Column("code_concept"), StringLength(10)]
        public string CodeConcept { get; set; }
        [Column("account_number"), StringLength(20), Required]
        public string AccountNumber { get; set; }
        [Column("unit_measurement"), StringLength(10), Required]
        public string UnitMeasurement { get; set; }
        [Column("description"), StringLength(150)]
        public string Description { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("id_debt"), Required]
        public int DebtId { get; set; }
        [NotMapped]
        public Debt Debt { get; set; }
        [Column("id_prepaid"), Required]
        public int PrepaidId { get; set; }
        [NotMapped]
        public Prepaid Prepaid { get; set; }
        [Column("id_order_sale"), Required]
        public int OrderSaleId { get; set; }
        [NotMapped]
        public OrderSale OrderSale { get; set; }
        [Required, Column("have_tax")]
        public bool HaveTax { get; set; }
        [Column("tax"), Required]
        public decimal Tax { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }

        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
