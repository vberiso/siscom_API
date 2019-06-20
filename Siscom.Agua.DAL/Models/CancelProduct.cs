using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Cancel_Product")]
    public class CancelProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_cancel_product")]
        public int Id { get; set; }
        [Column("account"), StringLength(50)]
        public string Account { get; set; }
        [Required, StringLength(100), Column("code_concept")]
        public string CodeConcept { get; set; }
        [Required, StringLength(5000), Column("name_concept")]
        public string NameConcept { get; set; }
        [Column("typeConcept"), StringLength(5), Required]
        public string TypeConcept { get; set; }
        [Required, Column("amount")]
        public decimal Amount { get; set; }
        [Column("request_date"), Required]
        public DateTime RequestDate { get; set; }
        [Column("requesterId"), StringLength(100), Required]
        public string RequesterId { get; set; }
        [Column("authorisation_date"), Required]
        public DateTime AuthorisationDate { get; set; }
        [Column("supervisorId"), StringLength(100), Required]
        public string SupervisorId { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
        [Required, StringLength(500), Column("motivo_cancelacion")]
        public string MotivoCancelacion { get; set; }
        [Column("debtId"), Required]
        public int DebtId { get; set; }
        [Column("orderSaleId"), Required]
        public int OrderSaleId { get; set; }

    }
}
