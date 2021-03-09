using Siscom.Agua.DAL.Models.ModelsProcedures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Division")]
    public class Division
    {
        public Division()
        {
            Products = new HashSet<Product>();
            OrderSale = new HashSet<OrderSale>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_division")]
        public int Id { get; set; }
        [Required, StringLength(150), Column("name")]
        public string Name { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("id_solution")]
        public int IdSolution { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<OrderSale> OrderSale { get; set; }
        public ICollection<AvailableProcedure> AvailableProcedures { get; set; }
    }
}
