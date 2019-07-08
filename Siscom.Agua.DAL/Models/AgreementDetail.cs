using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Agreement_Detail")]
    public class AgreementDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_agreement_detail")]
        public int Id { get; set; }
        [Column("folio"), Required, StringLength(50)]
        public string Folio { get; set; }
        [Column("register"), Required, StringLength(50)]
        public string Register { get; set; }
        [Column("taxable_base"), Required]
        public decimal TaxableBase { get; set; }
        [Column("ground"), Required]
        public decimal Ground { get; set; }
        [Column("built")]
        public decimal Built { get; set; }
        [Column("agreement_detail_date"), Required]
        public DateTime AgreementDetailDate { get; set; }
        [Column("last_update"), Required]
        public DateTime LastUpdate { get; set; }
        [Column("sector")]
        public Int16 Sector { get; set; }
        [Column("observation")]
        public string Observation { get; set; }
        [Column("manifest")]
        public Boolean Manifest { get; set; }
        [Column("catastral_key"), StringLength(50)]
        public string CatastralKey { get; set; }

        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
    }
}
