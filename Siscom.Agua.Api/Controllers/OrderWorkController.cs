
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

using Microsoft.AspNetCore.Http;


using System;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Siscom.Agua.Api.Model;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
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
                    .Include(x => x.TypeIntake)
                    .Include(x => x.TypeStateService)
                    .Include(x => x.OrderWork)
                        .ThenInclude(x => x.TechnicalStaff)
                    .Include(x => x.Clients)
                    .Include(x => x.Addresses)
                        .ThenInclude(x => x.Suburbs)
                            .ThenInclude(x => x.Towns)
                                .ThenInclude(x => x.States)
                                    .ThenInclude(x => x.Countries)
                    .First();

                return Ok(agreement);  
            }
            catch (Exception ex)
            {
                return Ok("");
            }
            
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

        [HttpGet("TypeOfReconnection/{TypeIntakeId}")]
        public async Task<IActionResult> GetParamsReconnection([FromRoute] int TypeIntakeId)
        {
            string[] array = null;
            List<ProductVM> products = new List<ProductVM>();
            try
            {
                switch (TypeIntakeId)
                {
                    case 1:
                        array = _context.SystemParameters.Where(x => x.Name == "RECO1").Select(x => x.TextColumn).FirstOrDefault().Split(",");
                        array.ToList().ForEach(x =>
                        {
                            products.Add(_context.TariffProducts.Include(y => y.Product).Where(y => y.ProductId == Convert.ToInt32(x)).Select(y => new ProductVM
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name,
                                Amount = y.Amount
                            }).FirstOrDefault());
                        });
                        break;
                    case 2:
                        array = _context.SystemParameters.Where(x => x.Name == "RECO2").Select(x => x.TextColumn).FirstOrDefault().Split(",");
                        array.ToList().ForEach(x =>
                        {
                            products.Add(_context.TariffProducts.Include(y => y.Product).Where(y => y.ProductId == Convert.ToInt32(x)).Select(y => new ProductVM
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name,
                                Amount = y.Amount
                            }).FirstOrDefault());
                        });
                        break;
                    case 3:
                        array = _context.SystemParameters.Where(x => x.Name == "RECO3").Select(x => x.TextColumn).FirstOrDefault().Split(",");
                        array.ToList().ForEach(x =>
                        {
                            products.Add(_context.TariffProducts.Include(y => y.Product).Where(y => y.ProductId == Convert.ToInt32(x)).Select(y => new ProductVM
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name,
                                Amount = y.Amount
                            }).FirstOrDefault());
                        });
                        break;
                    case 4:
                        array = _context.SystemParameters.Where(x => x.Name == "RECO1").Select(x => x.TextColumn).FirstOrDefault().Split(",");
                        array.ToList().ForEach(x =>
                        {
                            products.Add(_context.TariffProducts.Include(y => y.Product).Where(y => y.ProductId == Convert.ToInt32(x)).Select(y => new ProductVM
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name,
                                Amount = y.Amount
                            }).FirstOrDefault());
                        });
                        break;
                    case 5:
                        array = _context.SystemParameters.Where(x => x.Name == "RECO1").Select(x => x.TextColumn).FirstOrDefault().Split(",");
                        array.ToList().ForEach(x =>
                        {
                            products.Add(_context.TariffProducts.Include(y => y.Product).Where(y => y.ProductId == Convert.ToInt32(x)).Select(y => new ProductVM
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name,
                                Amount = y.Amount
                            }).FirstOrDefault());
                        });
                        break;
                    default:
                        return StatusCode(StatusCodes.Status400BadRequest, new { error = "Tipo de Toma no clasificada, favor de verificar" });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }
    }
}