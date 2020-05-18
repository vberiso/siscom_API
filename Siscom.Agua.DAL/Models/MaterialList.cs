using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class MaterialList
    {
        public MaterialList()
        {
            UnitMeasurements = new HashSet<UnitMeasurement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_material")]
        public int Id { get; set; }
        [Column("name"), StringLength(60)]
        public string Name { get; set; }
        [Column("code"), StringLength(25)]
        public string Code { get; set; }
        public bool IsActive { get; set; }

        public ICollection<UnitMeasurement> UnitMeasurements { get; set; }
        public ICollection<MaterialMovements> MaterialMovements { get; set; }
    }
}
