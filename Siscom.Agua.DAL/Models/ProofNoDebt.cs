using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{

    [Table("Proof_No_Debt")]
    public class ProofNoDebt
    {
        [Key]
        [Column("id_proof")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("folio")]
        public int Folio { get; set; }
        [Column("expedition_date")]
        public DateTime Expedition_date { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
        [Column("expiration_date")]
        public DateTime Expiration_date { get; set; }
        [Column("AgreementId")]
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }
        
    }
}
