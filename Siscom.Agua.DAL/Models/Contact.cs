using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Contact")]
    public class Contact
    {
        [Column("id_contact"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("phone_number"), StringLength(50), Required]
        public string PhoneNumber { get; set; }
        [Column("type_number"), Required]
        public string TypeNumber { get; set; }
        [Column("is_active"), Required]
        public int IsActive { get; set; }

        public Client Client { get; set; }
    }
}
