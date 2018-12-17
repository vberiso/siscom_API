using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("System_Parameters")]
    public class SystemParameters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_system_parameters")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }
        [Column("start_date"), Required]
        public DateTime StartDate { get; set; }
        [Column("end_date"), Required]
        public DateTime EndDate { get; set; }
        [Required, Column("is_active")]
        public bool IsActive { get; set; }
        [Required, Column("type_column")]
        public Int16 TypeColumn { get; set; }
        [Column("number_column")]
        public decimal NumberColumn { get; set; }
        [Column("text_column")]
        public string TextColumn { get; set; }
        [Column("date_column")]
        public DateTime DateColumn { get; set; }
    }
}
