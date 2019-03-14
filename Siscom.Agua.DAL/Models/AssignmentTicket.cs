using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Assignment_Ticket")]
    public class AssignmentTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_assignment_ticket")]
        public int Id { get; set; }        
        [Required, Column("date_assignment")]
        public DateTime AssignmentDate { get; set; }      
        [Required, Column("folio")]
        public int Folio { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Required]
        public string TransitPoliceId { get; set; }
        public TransitPolice TransitPolice { get; set; }
    }
}
