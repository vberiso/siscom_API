using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("inspection_fine")]
    public class InspectionFine
    {
        [Key]
        [Column("id_inspection_fine")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name"), StringLength(100)]
        public string Name { get; set; }
        [Column("from")]
        public int From { get; set; }
        [Column("until")]
        public int Until { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("have_tax")]
        public bool HaveTax { get; set; }
    }
}
