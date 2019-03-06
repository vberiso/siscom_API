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
            BreachWarranties = new HashSet<BreachWarranty>();
            BreachDetails = new HashSet<BreachDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach")]
        public int Id { get; set; }
        [Column("series"),Required,StringLength(50)]
        public string Series { get; set; }
        [Required, Column("folio"), StringLength(10)]
        public string Folio { get; set; }
        [Required, Column("date_capture")]
        public DateTime CaptureDate { get; set; }
        [Column("place"), Required, StringLength(256)]
        public string Place { get; set; }
        [Column("sector"), StringLength(50)]
        public string Sector { get; set; }
        [Column("zone"), StringLength(50)]
        public string Zone { get; set; }
        [Column("car"), Required, StringLength(100)]
        public string Car { get; set; }
        [Column("type_car"), Required, StringLength(100)]
        public string TypeCar { get; set; }
        [Column("service"), Required, StringLength(100)]
        public string Service { get; set; }
        [Column("color"), Required, StringLength(100)]
        public string Color { get; set; }
        [Column("licenseplate"), Required, StringLength(50)]
        public string LicensePlate { get; set; }
        [Column("reason"), Required, StringLength(256)]
        public string Reason { get; set; }
        [Column("judge"), Required]
        public decimal Judge { get; set; }

        [NotMapped]
        public string DescriptionType { get; set; }

        [NotMapped]
        public string DescriptionStatus { get; set; }

        [Required, Column("date_breach")]
        public DateTime DateBreach { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("id_assignment_ticket"),  Required]
        public int AssignmentTicketId { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int TaxUserId { get; set; }
        public TaxUser TaxUser { get; set; }

        public ICollection<BreachWarranty> BreachWarranties { get; set; }
        public ICollection<BreachDetail> BreachDetails { get; set; }
    }
}
