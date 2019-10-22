using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Transaction_Cancelation_Request")]
    public class TransactionCancellationRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_transaction_cancelation_request")]
        public int Id { get; set; }

        public string Status { get; set; }
        public DateTime DateRequest { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Reason { get; set; }

        public string Manager { get; set; }
        public DateTime DateAuthorization { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string ManagerObservation { get; set; }
        [Column("Key_Firebase"), StringLength(50)]
        public string KeyFirebase { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
