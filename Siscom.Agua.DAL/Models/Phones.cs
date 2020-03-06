using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("phone")]
    public class Phones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_Phone")]
        public int Id { get; set; }
        [Column("id_device"), StringLength(30)]
        public string IdDevice { get; set; }
        [Column("phone_number"), StringLength(20), DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Column("assigned_user"), StringLength(150)]
        public string AssignedUser { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("register_date")]
        public DateTime RegisterDate { get; set; }
        [Column("last_update_date")]
        public DateTime LastUpdateDate { get; set; }
        [Column("brand"), StringLength(25)]
        public string Brand { get; set; }
        [Column("model"), StringLength(25)]
        public string Model { get; set; }
    }
}
