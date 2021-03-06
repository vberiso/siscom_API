using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    [Table("Order_Citizen_Procedure")]
    public class OrderCitizenProcedure
    {

        [Key]
        public int OrderSaleId { get; set; }
        public OrderSale OrderSale { get; set; }

        [Key]
        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }

        [Required]
        public String UserId { get; set; }

        //no estoy seguro de que este vaya aqui
        public int DivisionId { get; set; }
        public Division Division { get; set; }
    }
}
