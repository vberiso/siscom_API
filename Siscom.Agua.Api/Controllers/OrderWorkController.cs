using System;
using System.Collections.Generic;
using System.Data;
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

        [HttpGet("OrderWorks/{id?}")]
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
            try
            {
                var OrderWork = _context.OrderWorks.Where(x => x.Id.ToString() == id).First();
                var agreement = _context.Agreements.Where(a => a.Id == OrderWork.AgrementId)
                    .Include(x => x.OrderWork)
                        .ThenInclude(x => x.TechnicalStaff)
                    .Include(x => x.Clients)
                    .Include(x => x.Addresses)
                        .ThenInclude(x => x.Suburbs)
                            .ThenInclude(x => x.Towns)
                                .ThenInclude(x => x.States)
                                    .ThenInclude(x => x.Countries)
                                        .First();

                //var OrderWork = _context.OrderWorks.Where(x => x.Id.ToString() == id)
                //    .Include(x => x.Agreement) 
                //    .ThenInclude(x => x.Clients)
                //    .Tnclude(x => x.Address)
                //        .ThenInclude(x => x.Suburbs)
                //            .ThenInclude(x => x.Town)
                //                .ThenInclude(x => x.State)
                //                    .ThenInclude(x => x.Country)
                //                        .First();

              
                return Ok(agreement);
                
                
            }
            catch (Exception ex)
            {
                return Ok("");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderWorksById([FromRoute] int id)
        {
            var order = await _context.OrderWorks.FindAsync(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
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
                        Observation = data["Observation"].ToString(),
                        Folio = "f",
                        Activities = data["Activities"].ToString()


                    };
                    _context.OrderWorks.Add(OrderWork);

                   
                    _context.SaveChanges();
                    var Status = new OrderWorkStatus()
                    {
                        IdStatus = "EOT01",
                        OrderWorkId = OrderWork.Id,
                        User = data["applicant"].ToString(),
                        OrderWorkStatusDate = DateTime.Now

                    };
                    _context.OrderWorkStatus.Add(Status);


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