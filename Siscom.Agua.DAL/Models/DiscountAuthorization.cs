using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Discount_Authorization")]
    public class DiscountAuthorization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_discount_authorization")]
        public int Id { get; set; }
        [Column("request_date"), Required]
        public DateTime RequestDate { get; set; }
        [Column("authorization_date")]
        public DateTime AuthorizationDate { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }
        [Column("amount_discount"), Required]
        public decimal AmountDiscount { get; set; }
        [Column("discount_percentage")]
        public Int16 DiscountPercentage { get; set; }
        [Required,Column("account"), StringLength(50)]
        public string Account { get; set; }
        [Required, StringLength(30), Column("folio")]
        public string Folio { get; set; }
        [Column("Key_Firebase"), StringLength(50)]
        public string KeyFirebase { get; set; }
        [Column("id_origin")]
        public int IdOrigin { get; set; }       
        [Required, StringLength(5), Column("type")]
        public string Type { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("observation")]
        public string Observation { get; set; }
        [Column("branch_office"), StringLength(30), Required]
        public string BranchOffice { get; set; }

        public string UserAuthorizationId { get; set; }

        [Required]
        public string UserRequestId { get; set; }
        public ApplicationUser UserRequest { get; set; }
        public ICollection<DiscountAuthorizationDetail> DiscountAuthorizationDetails { get; set; }
    }
}
