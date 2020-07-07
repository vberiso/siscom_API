using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("valvula_control")]
    public class ValvulaControl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_valvula_control")]
        public int Id { get; set; }
        [Column("description"), StringLength(150)]
        public string Description  { get; set; }
        [Column("reference"), StringLength(300)]
        public string Reference { get; set; }
        [Column("latitude"), StringLength(20)]
        public string Latitude { get; set; }
        [Column("longitude"), StringLength(20)]
        public string Longitude { get; set; }
        [Column("type"), StringLength(5)]
        public string Type { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("diameter"), StringLength(20)]
        public string Diameter { get; set; }
        [Column("hydraulic_circuit"), StringLength(50)]
        public string HydraulicCircuit { get; set; }
        [Column("physical_state")]
        public string PhysicalState { get; set; }
        [Column("actual_state")]
        public string ActualState { get; set; }
        [Column("last_service_date")]
        public DateTime LastServiceDate { get; set; }

        public ICollection<ValveIncident> ValveIncidents { get; set; }
        public ICollection<ValveOperation> ValveOperations { get; set; }

    }
}
