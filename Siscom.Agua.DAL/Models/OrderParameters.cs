using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Order_Work_Parameters")]
    public class OrderParameters
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_order_work_parameters")]
        public int Id { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }


        [Column("start_date"), Required]
        public DateTime startDate { get; set; }


        [Column("end_date"), Required]
        public DateTime EndDate { get; set; }


        [Column("is_active"), Required]
        public bool IsActive { get; set; }


        [Column("is_agreement"), Required]
        public bool isAgreement { get; set; }

        [Column("type_order"), Required]
        public string TypeOrder { get; set; }


        [Column("status_order"), Required]
        public string StatusOrder { get; set; }

        [Column("TypeIntakeId")]
        public string TypeIntakeId { get; set; }

        [Column("code_concept")]
        public string CodeConcept { get; set; }

    }
}
