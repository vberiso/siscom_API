using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{ 
    [Table("order_work_detail")]
    public class OrderWorkDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("name"), StringLength(40)]
        public string Name { get; set; }
        [Column("type"), StringLength(5)]
        public string Type { get; set; }
        [Column("value"), StringLength(150)]
        public string Value { get; set; }
        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }
    }
}
