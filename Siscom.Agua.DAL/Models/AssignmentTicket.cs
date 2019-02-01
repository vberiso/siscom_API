using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siscom.Agua.DAL.Models
{
    [Table("Assignment_Ticket")]
    public class AssignmentTicket
    {
        public AssignmentTicket()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_breach_Warranty")]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Required, Column("assignmentdate")]
        public DateTime AssignmentDate { get; set; }

        [Column("serie"), Required, StringLength(30)]
        public string Serie { get; set; }

        [Column("folio"), Required, StringLength(30)]
        public string Folio { get; set; }

        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }




        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
