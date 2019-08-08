using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Facturama;
using Facturama.Services;
using Siscom.Agua.DAL;
using Siscom.Agua.Api.Data;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FacturacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacturacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderSales
        [HttpGet("ValidateFrom/{ini}/{fin}")]
        public async Task<IActionResult> GetOrderSales([FromRoute] string ini, [FromRoute] string fin)
        {
            DateTime FechaIni = new DateTime(int.Parse(ini.Split("-")[0]), int.Parse(ini.Split("-")[1]), int.Parse(ini.Split("-")[2]));
            DateTime FechaFin = new DateTime(int.Parse(ini.Split("-")[0]), int.Parse(ini.Split("-")[1]), int.Parse(ini.Split("-")[2]), 23, 59, 59);
            var tmp = _context.TaxReceipts.Where(x => x.TaxReceiptDate > FechaIni && x.TaxReceiptDate < FechaFin && x.Status == "ET001").ToList();

            RequestsAPI RequestsFacturama = new RequestsAPI("https://api.facturama.mx/");
            int count = 0, refresh = 0;
            try
            {                
                foreach (var item in tmp)
                {
                    Facturama.Models.Response.Cfdi cfdiGet = new Facturama.Models.Response.Cfdi();
                    if (!string.IsNullOrEmpty(item.IdXmlFacturama))
                    {
                        var resultado = await RequestsFacturama.SendURIAsync(string.Format("api-lite/cfdis/{0}", item.IdXmlFacturama), HttpMethod.Get, "gfdsystems", "gfds1st95");
                        cfdiGet = JsonConvert.DeserializeObject<Facturama.Models.Response.Cfdi>(resultado);
                    }                    

                    if (cfdiGet.Items == null)
                    {
                        item.Status = "ET003";
                        _context.Entry(item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        refresh++;
                    }
                    count++;
                }
            }
            catch(Exception ex)
            {                
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Problemas al consultar los registros {0}", ex.Message) });
            }
            return Ok(string.Format("Registros revisados: {0} de {1}, se actualizaron: {2}", count, tmp.Count, refresh ));
        }

    }
}