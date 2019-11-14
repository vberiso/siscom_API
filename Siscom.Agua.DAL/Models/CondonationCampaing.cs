using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class CondonationCampaing
    {
        [Key]
        [Column("id_condonation_campaign"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        [Column("alias"), StringLength(200)]
        public string Alias { get; set; }

        [Column("tipes"), StringLength(100)]
        public string Types { get; set; }

        [Column("codes"), StringLength(200)]
        public string Codes { get; set; }

        [Column("percentage")]
        public Int16 Percentage { get; set; }

        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        
        [Column("end_date")]
        public DateTime EndDate { get; set; }
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("condonation_from")]
        public DateTime CondonationFrom { get; set; }
        [Column("condonation_until")]
        public DateTime CondonationUntil { get; set; }
    }
}
