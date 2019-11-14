using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
   public class PagosAnuales
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_pagos_anuales")]
        public int Id { get; set; }

        [Column("AgreementId")]
        public int AgreementId { get; set; }

        [Column("DebtId"), Required]
        public int DebtId { get; set; }

        [Column("DateDebt")]
        public DateTime DateDebt { get; set; }

        [Column("Status"), Required]
        public string Status { get; set; }

    }
}
