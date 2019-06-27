using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("accounting_status")]
    public class AccountingStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_accounting_status")]
        public int Id { get; set; }
        [Column("request_date")]
        public DateTime RequestDate { get; set; }
        [Column("POI_error")]
        public int POIError { get; set; }
        [Column("POC_mensage_error"), StringLength(500)]
        public string POC_MensageError { get; set; }
        [Column("accounting_code_id")]
        public int AccountingCodeId { get; set; }
    }
}
