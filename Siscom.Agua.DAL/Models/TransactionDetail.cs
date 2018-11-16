﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Transaction_Detail")]
    public class TransactionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_transaction_detail")]
        public int Id { get; set; }
        [Column("code_concept"), StringLength(10)]
        public string CodeConcept { get; set; }
        [Column("description"), StringLength(30)]
        public string Description { get; set; }
        [Column("amount"), Required]
        public double amount { get; set; }

        public Transaction Transaction { get; set; }
    }
}
