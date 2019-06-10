using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class BenefitedCampaign
    {
        [Key]
        [Column("id_discount_campaign")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("agreementId"), Required]
        public int AgreementId { get; set; }
        [Column("discount_campaignId"), Required]
        public int DiscountCampaignId { get; set; }
        [Column("name_camping"), StringLength(75), Required]
        public string NameCamping { get; set; }
        public DateTime ApplicationDate { get; set; }
        public decimal AmountDiscount { get; set; }
    }
}
