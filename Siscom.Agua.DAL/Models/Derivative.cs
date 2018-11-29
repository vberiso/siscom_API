using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Derivative")]
    public class Derivative
    {
        [Key]
        [Column("id_derivative"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public Agreement Agreement { get; set; }
        public int AgreementDerivative { get; set; }
    }
   
}
