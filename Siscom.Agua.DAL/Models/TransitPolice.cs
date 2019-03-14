using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class TransitPolice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_transit_police")]
        public int Id { get; set; }
        [Column("name"), Required, StringLength(100)]
        public String Name { get; set; }
        [Column("phone_number"), StringLength(50)]
        public String PhoneNumber { get; set; }
        [Column("email"), StringLength(150)]
        public String EMail { get; set; }
        [Column("address"), StringLength(300)]
        public String Address { get; set; }
        [Column("plate"), StringLength(50)]
        public String Plate { get; set; }        
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
