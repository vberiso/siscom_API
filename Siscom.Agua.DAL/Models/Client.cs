using Siscom.Agua.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Client")]
    public class Client
    {

        public Client()
        {
            Contacts = new HashSet<Contact>();
        }

        [Column("id_client"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column("name"), StringLength(200), Required]
        public string Name { get; set; }
        [Column("last_name"), StringLength(80), Required]
        public string LastName { get; set; }
        [Column("second_last_name"), StringLength(80), Required]
        public string SecondLastName { get; set; }
        [Column("rfc"), StringLength(17)]
        public string RFC { get; set; }
        [Column("curp"), StringLength(18)]
        public string CURP { get; set; }
        [Column("ine"), StringLength(20)]
        public string INE { get; set; }
        [Column("email"), StringLength(150)]
        public string EMail { get; set; }
        [Column("type_user"), StringLength(5), Required]
        public string TypeUser { get; set; }
   
        [ForeignKey("Agreement")]
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public ICollection<Contact> Contacts { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Name, SecondLastName, LastName);
        }

    }
}
