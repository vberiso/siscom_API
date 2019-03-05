﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Discount_Campaign")]
    public class DiscountCampaign
    {
        [Key]
        [Column("id_discount_campaign"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name"), StringLength(50), Required]
        public string Name { get; set; }
        
        [Column("percentage")]
        public Int16 Percentage { get; set; }
        
        [Column("is_variable")]
        public bool IsVariable { get; set; }

        [Column("profile")]
        public bool profile { get; set; }

        [Column("is_active"), Required]
        public bool IsActive { get; set; }
    }
}
