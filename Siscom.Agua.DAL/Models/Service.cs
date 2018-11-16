using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Service")]
    public class Service
    {
        public Service()
        {
            AgreementServices = new HashSet<AgreementService>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_service")]
        public int Id { get; set; }
        [Required, StringLength(25), Column("name")]
        public string Name { get; set; }

        public ICollection<AgreementService> AgreementServices { get; set; }
    }
}
