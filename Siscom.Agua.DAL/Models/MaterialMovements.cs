using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Odbc;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("material_movements")]
    public class MaterialMovements
    {
        [Column("orderWork_id"), Required]
        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }

        [Column("materialList_id"), Required]
        public int MaterialListId { get; set; }
        public MaterialList MaterialList { get; set; }

        [Column("movement_date"), Required]
        public DateTime MovementDate { get; set; }
        [Column("type"), StringLength(5)]
        public string Type { get; set; }
        [Column("quantity")]
        public decimal Quantity { get; set; }
    }
}
