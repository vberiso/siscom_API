using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class DataReportes
    {
        public string FechaIni { get; set; }
        public string FechaFin { get; set; }
        public string CajeroId { get; set; }
        public string CajeroNombre { get; set; }
        public string CajeroAPaterno { get; set; }
        public string CajeroAMaterno { get; set; }
        public string Oficinas { get; set; }
        public string statusIFB { get; set; }
        public Boolean pwaFiltrarPorContrato { get; set; }
        public string pwaColonias { get; set; }
        public string pwaRutas { get; set; }
        public string pwaServicios { get; set; }
        public string pwaTomas { get; set; }
    }
}
