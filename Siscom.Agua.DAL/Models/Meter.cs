using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Meter")]
    public  class Meter
    {
        [Key]
        [Column("id_meter"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Column("brand"), StringLength(50), Required]
        public string Brand { get; set; }
        [Column("model"), StringLength(50), Required]
        public string Model { get; set; }
        [Column("consumption"), StringLength(10)]
        public string Consumption { get; set; }
        [Column("install_date"), Required]
        public DateTime InstallDate { get; set; }
        [Column("deinstall_date"), Required]
        public DateTime DeinstallDate { get; set; }
        [Column("serial"), StringLength(20), Required]
        public string Serial { get; set; }
        [Column("wheels"), StringLength(1), Required]
        public string Wheels { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public Agreement Agreement { get; set; }
    }
}
