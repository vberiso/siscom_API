using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models.ModelsProcedures
{
    class OrderCitizenProcedure
    {

        [Key]
        public int OrderSaleID { get; set; }
        public OrderSale OrderSale { get; set; }

        [Key]
        public int CitizenProcedureId { get; set; }
        public CitizenProcedure CitizenProcedure { get; set; }

        [Column("UserId")]
        public String UserId { get; set; }

        //no estoy seguro de que este vaya aqui
        public int DivisionId { get; set; }
        public Division Division { get; set; }
    }
}
