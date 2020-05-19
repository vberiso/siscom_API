using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class UnitMeasurement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_unit_measurement")]
        public int Id { get; set; }
        [Column("acronym"), StringLength(10)]
        public string Acronym { get; set; }
        [Column("description"), StringLength(60)]
        public string Description { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        public int MaterialListId { get; set; }
        public MaterialList MaterialList { get; set; }
    }
}
