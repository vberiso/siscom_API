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
        [Column("serie"), StringLength(50)]
        public string Serie { get; set; }
        [Required, Column("folio"), StringLength(10)]
        public int Folio { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
