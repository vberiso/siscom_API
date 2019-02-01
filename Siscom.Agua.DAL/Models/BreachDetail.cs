using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach_Detail")]
    public class BreachDetail
    {
        public BreachDetail()
        {
            BreachList = new HashSet<BreachList>();

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_detail")]
        public int Id { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("aplicationdays")]
        public DateTime AplicationDays { get; set; }

        [Column("amount"), Required, StringLength(30)]
        public string Amount { get; set; }

        [Column("porcentbreach"), Required, StringLength(30)]
        public string PercentBreach { get; set; }

        [Column("bonification"), Required, StringLength(30)]
        public string Bonification { get; set; }


        public string BreachId { get; set; }
        public Breach  Breach { get; set; }

        
        public ICollection<BreachList> BreachList { get; set; }





    }
}
