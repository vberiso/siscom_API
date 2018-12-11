﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Prepaid")]
    public class Prepaid
    {
        public Prepaid()
        {
            PrepaidDetails = new HashSet<PrepaidDetail>();
            DebtPrepaids = new HashSet<DebtPrepaid>();
        }

        [Key]
        [Column("id_prepaid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("prepaid_date"), Required]
        public DateTime PrepaidDate { get; set; }
        [Column("amount"), Required]
        public double Amount { get; set; }
        [Column("accredited"), Required]
        public double Accredited { get; set; }
        [Column("status"), StringLength(5), Required]
        public string Status { get; set; }

        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public ICollection<PrepaidDetail> PrepaidDetails { get; set; }
        public ICollection<DebtPrepaid> DebtPrepaids { get; set; }
    }
}
