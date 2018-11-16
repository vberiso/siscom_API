using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Agreement_Service")]
    public class AgreementService
    {

        [Column("id_service"), Required]
        public int IdService { get; set; }
        public Service Service { get; set; }

        [Column("id_agreement"), Required]
        public int IdAgreement { get; set; }
        public Agreement Agreement { get; set; }

        [Column("agreement_date"), Required]
        public DateTime DateAgreement { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
        
    }
}
