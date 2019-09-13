﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Technical_Staff")]
    public class TechnicalStaff
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_technical_staff")]
        public int Id { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }


        [Column("phone"), Required]
        public string Phone { get; set; }



        [Column("TechnicalRoleId"), Required]
        public int TechnicalRoleId { get; set; }
        public TechnicalRole TechnicalRole { get; set; }


        [Column("TechnicalTeamId"), Required]
        public int TechnicalTeamId { get; set; }
        public TechnicalTeam TechnicalTeam { get; set; }




        [Column("is_active"), Required]
        public bool IsActive { get; set; }


    }
}