using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("valve_incident")]
    public class ValveIncident
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_anomalies_of_valve")]

        public int Id { get; set; }
        [Column("type"), StringLength(5)]
        public string Type { get; set; }
        [Column("anomalies_date")]
        public DateTime AnomaliesDate { get; set; }
        [Column("observations"), StringLength(300)]
        public string Observations { get; set; }
        [Column("atention_date")]
        public DateTime AtentionDate { get; set; }
        [Column("work_description"), StringLength(300)]
        public string WorkDescription { get; set; }

        public int ValvulaControlId { get; set; }
        public ValvulaControl ValvulaControl { get; set; }

    }
}
