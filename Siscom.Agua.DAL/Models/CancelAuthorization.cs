using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Cancel_Authorization")]
    public class CancelAuthorization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_cancel_authorization")]
        public int Id { get; set; }
        [Column("id_terminal_user"), Required]
        public int TerminalUserId { get; set; }
        [Column("id_branch_office")]
        public int BranchOfficeId { get; set; }
        [Column("id_transaction"), Required]
        public int TransactionId { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }
        [Column("id_user")]
        public string UserId { get; set; }
        [Column("date_authorization")]
        public DateTime AuthorizationDate { get; set; }
    }

}
