using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Breach")]
    public class Breach
    {
        public Breach()
        {
            TaxUser = new HashSet<TaxUser>();
            BreachWarranty = new HashSet<BreachWarranty>();
            BreachDetail = new HashSet<BreachDetail>();

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach")]
        public int Id { get; set; }

        [Column("series"),Required,StringLength(52)]
        public string Series { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("capture")]
        public DateTime Capture { get; set; }

        [Column("place"), Required, StringLength(256)]
        public string Place { get; set; }

        [Column("sector"), Required, StringLength(50)]
        public string Sector { get; set; }

        [Column("zone"), Required, StringLength(50)]
        public string Zone { get; set; }

        [Column("car"), Required, StringLength(100)]
        public string Car { get; set; }

        [Column("type"), Required, StringLength(100)]
        public string Type { get; set; }

        [Column("service"), Required, StringLength(100)]
        public string Service { get; set; }

        [Column("color"), Required, StringLength(100)]
        public string Color { get; set; }

        [Column("licenseplate"), Required, StringLength(50)]
        public string LicensePlate { get; set; }

        [Column("reason"), Required, StringLength(256)]
        public string Reason { get; set; }

        [Column("judge"), Required, StringLength(100)]
        public string Judge { get; set; }



        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("datebreach")]
        public DateTime DateBreach { get; set; }

        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }


        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        public ICollection<TaxUser> TaxUser { get; set; }
        public ICollection<BreachWarranty> BreachWarranty { get; set; }
        public ICollection<BreachDetail> BreachDetail { get; set; }





    }
}
