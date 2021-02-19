using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.Promos
{
    public class PromotionCaja
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombrePublico { get; set; }
        public string DescripcionPublico { get; set; }
        public string ObservacionFactura { get; set; }
        public DateTime VigenciaInicio { get; set; }
        public DateTime VigenciaFinal { get; set; }
        //public int PorcentajeDescuento { get; set; }
        public List<int> TiposToma { get; set; }
        public bool AplicaEnOnline { get; set; }
        public int PromocionAño { get; set; }
        public int PromocionMesIncio { get; set; }
        public int PromocionMesFinal { get; set; }
        public List<PeriodsDiscountCaja> PromocionAplicar { get; set; }
        public bool BorrarDeudaAñoPromocion { get; set; }
        public List<DescuentosCaja> Descuentos { get; set; }
        public List<DescuentosCaja> Condonaciones { get; set; }
    }
}
