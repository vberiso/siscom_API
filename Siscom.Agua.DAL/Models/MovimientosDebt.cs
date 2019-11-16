using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class MovimientosDebt
    {
        [Column("id_movimientos_debt"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int DebtIdFather { get; set; }
        public string StatusOrigin { get; set; }
        public string StatusResult { get; set; }
        public int DebtIdSon { get; set; }
        public string StatusDebtIdSon { get; set; }
    }
}
