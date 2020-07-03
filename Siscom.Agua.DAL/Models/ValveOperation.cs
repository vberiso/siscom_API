using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("valve_operation")]
    public class ValveOperation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_valve_operations")]
        public int Id { get; set; }
        public DateTime OperationStart { get; set; }
        public DateTime OperationEnd { get; set; }
        public string OperationType { get; set; }
        public int ValvulaControlId { get; set; }
        public ValvulaControl ValvulaControl { get; set; }
    }
}
