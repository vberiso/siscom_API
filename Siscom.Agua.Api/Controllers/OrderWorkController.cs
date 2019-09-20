using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderWorkController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public OrderWorkController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("OrderWorks")]
        public async Task<IActionResult> GetOrderWorks([FromQuery] string id = null, [FromQuery] string status = null, [FromQuery] string folio = null, [FromQuery] string type = null)
        {
            if (id == null)
            {

                System.Linq.IQueryable<OrderWork> query = _context.OrderWorks;
                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (folio != null)
                {
                    query = query.Where(x => x.Folio == folio);
                }
                if (type != null)
                {
                    query = query.Where(x => x.Type == type);
                }
                var OrderWorks = query.ToList();

                return Ok(OrderWorks);
            }
            var OrderWork = _context.OrderWorks.Where(x => x.Id.ToString() == id)
                
                .First();
            return Ok(OrderWork);
        }

        [HttpPost("OrderWorks")]
        public async Task<IActionResult> Create([FromBody] object collection)
        {
            int ApplyIds = 0;
            var data = JObject.Parse(collection.ToString());
            try
            {
                string[] ids = JsonConvert.DeserializeObject<string[]>(data["ids"].ToString());


                OrderWork OrderWork = null;
                foreach (string id in ids)
                {
                    OrderWork = new OrderWork()
                    {
                        AgrementId = data["isAgreement"].ToString() == "1" ? int.Parse(id) : 0,
                        TaxUserId = data["isAgreement"].ToString() == "0" ? int.Parse(id) : 0,
                        DateOrder = DateTime.Now,
                        Applicant = data["applicant"].ToString(),
                        Type = data["typeOrder"].ToString(),
                        Status = "EOT01",
                        Observation = "Orden de trabajo",
                        Folio = "f",
                        Activities = "Orden de trabajo"


                    };
                    _context.OrderWorks.Add(OrderWork);
                    _context.SaveChanges();
                    ApplyIds++;


                }

                return StatusCode(StatusCodes.Status200OK, new { msg = "Orden generada correctamente", id = OrderWork.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message, msg = "Solo se pudieron generar las primeras " + ApplyIds + " ordenes" });
            }
        }


        [HttpPost("OrderWorks/update/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] OrderWork OrderWork)
        {
            try
            {
                _context.OrderWorks.Update(OrderWork);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Orden actualizada correctamente", id = OrderWork.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }

        [HttpPost("OrderWorks/UpdateStatus/{id}/{status}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] string status)
        {
            try
            {
                var OrderWork = _context.OrderWorks.Where(x => x.Id == id).First();
                OrderWork.Status = status;
                _context.OrderWorks.Update(OrderWork);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Status actualizado correctamente", id = OrderWork.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }
    }
}