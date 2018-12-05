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
            Tariffs = new HashSet<Tariff>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_service")]
        public int Id { get; set; }
        [Required, StringLength(50), Column("name")]
        public string Name { get; set; }
        [Required, Column("order")]
        public Int16 Order { get; set; }       
        [Required, Column("is_commercial")]
        public bool IsCommercial { get; set; }      
        [Required, Column("is_active")]
        public bool IsActive { get; set; }

        public ICollection<AgreementService> AgreementServices { get; set; }
        public ICollection<Tariff> Tariffs { get; set; }
    }
}
