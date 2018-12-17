using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{  
    [Table("Prepaid_Detail")]
    public class PrepaidDetail
    {
        public PrepaidDetail()
        {
          
            DebtPrepaids = new HashSet<DebtPrepaid>();
        }

        [Key]
        [Column("id_drepaid_detail"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("prepaid_detail_date"), Required]
        public DateTime PrepaidDetailDate { get; set; }
        [Column("amount"), Required]
        public decimal Amount { get; set; }       
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        public int PrepaidId { get; set; }
        public Prepaid Prepaid { get; set; }

        public ICollection<DebtPrepaid> DebtPrepaids { get; set; }
    }
}
