using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("INPC")]
    public class INPC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_inpc")]
        public int Id { get; set; }
        [Column("year"), Required]
        public Int16 Year { get; set; }
        [Column("month"), Required]
        public Int16 Month { get; set; }
        [Column("value"), Required]
        public decimal Value { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
    }
}
