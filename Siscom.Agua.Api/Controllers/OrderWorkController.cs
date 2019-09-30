﻿
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
using Siscom.Agua.Api.Enums;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[ApiController]
    //[Authorize]
    public class OrderWorkController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public OrderWorkController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("OrderWorks/{id?}")]
        public async Task<IActionResult> GetOrderWorks([FromQuery] string id = null, [FromQuery] string status = null, [FromQuery] string folio = null, [FromQuery] string type = null, [FromQuery] string fecha = null, [FromQuery] string IsMasivo= null, [FromQuery] bool withOrder =false)
        {
            if (id == null)
            {
                System.Linq.IQueryable<OrderWork> query = _context.OrderWorks;
                if (folio == null)
                {
                    if (fecha != null)
                    {
                        fecha = DateTime.Parse(fecha).ToString("dd-MM-yyyy");
                        query = query.Where(x => x.DateOrder.ToString("dd-MM-yyyy") == fecha);
                    }
                    if (status != null)
                    {
                        query = query.Where(x => x.Status == status);
                    }

                    if (type != null)
                    {
                        query = query.Where(x => x.Type == type);
                    }
                    if(IsMasivo != null)
                    {
                        query = query.Where(x => x.Status == "EOT01");
                    }
                }
                else
                {
                    query = query.Where(x => x.Folio == folio);
                }
                query = query.Include(x => x.Agreement)

                        .OrderBy(x => x.Folio);

                var OrderWorks = query.ToList();
               
                return Ok(OrderWorks);
            }
            try
            {

                var OrderWork = _context.OrderWorks.Where(x => x.Id.ToString() == id)
                    .Include(x=>x.OrderWorkReasonCatalogs)
                    .ThenInclude(x => x.ReasonCatalog)
                    .First();
                var agreement = _context.Agreements.Where(a => a.Id == OrderWork.AgrementId)

                    .Include(x => x.TypeIntake)
                    .Include(x => x.TypeConsume)
                    .Include(x => x.OrderWork)
                        .ThenInclude(x => x.TechnicalStaff)
                        
                    .Include(x => x.Clients)
                    .Include(x => x.Addresses)
                        .ThenInclude(x => x.Suburbs)
                            .ThenInclude(x => x.Towns)
                                .ThenInclude(x => x.States)
                                    .ThenInclude(x => x.Countries)
                    .First();
                if (withOrder)
                {
                    return Ok(new List<object>() { agreement, OrderWork });
                }
                return Ok(agreement);
            }
            catch (Exception ex)
            {
                return Ok("");
            }

        }

        [HttpPost("OrderWorks/GetByIds")]
        public async Task<IActionResult> GetListOrderWorks([FromBody] List<int> list)
        {
            List<Agreement> agreements = new List<Agreement>();
            try
            {
                foreach (var item in list)
                {
                    var OrderWork = _context.OrderWorks.Where(x => x.Id == item).First();
                    var agreement = _context.Agreements.Where(a => a.Id == OrderWork.AgrementId)
                        .Include(x => x.TypeIntake)
                        .Include(x => x.TypeConsume)
                        .Include(x => x.OrderWork)
                            .ThenInclude(x => x.TechnicalStaff)
                        .Include(x => x.Clients)
                        .Include(x => x.Addresses)
                            .ThenInclude(x => x.Suburbs)
                                .ThenInclude(x => x.Towns)
                                    .ThenInclude(x => x.States)
                                        .ThenInclude(x => x.Countries)
                        .First();
                    agreements.Add(agreement);
                }
              

                return Ok(agreements);
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

        [HttpPost("OrderWorks/update/{id}/{user?}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromRoute] string user, [FromBody] OrderWork OrderWork)
        {
            try
            {
                var status = _context.OrderWorks.Where(x => x.Id == id).Select(x => x.Status).First();
                if (status != OrderWork.Status)
                {
                    var Status = new OrderWorkStatus()
                    {
                        IdStatus = OrderWork.Status,
                        OrderWorkId = OrderWork.Id,
                        User = user,
                        OrderWorkStatusDate = DateTime.Now

                    };
                    _context.OrderWorkStatus.Add(Status);
                }

                _context.OrderWorks.Update(OrderWork);
                //_context.Entry(OrderWork).State = EntityState.Modified;
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

        [HttpPost("AsignarStaffMasivo/{stafId}/{dateStimate}")]
        public async Task<IActionResult> AsignarStaffMasivo([FromRoute] int stafId, [FromRoute] string dateStimate, [FromBody] List<int> orderWorkIds)
        {
            try
            {
                OrderWork orderWOrk = null;
                orderWorkIds.ForEach(x =>
                {
                    orderWOrk = _context.OrderWorks.Where(order => order.Id == x && order.Status == "EOT01").FirstOrDefault();
                    if (orderWOrk != null)
                    {
                        orderWOrk.TechnicalStaffId = stafId;
                        orderWOrk.DateStimated = DateTime.Parse(dateStimate);
                        
                        orderWOrk.Status = "EOT02";
                        _context.OrderWorks.Update(orderWOrk);
                        _context.SaveChanges();
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { error = ex.Message });
            }
            
            return Ok(new { msg = "Operación exitosa" });
        }

        [HttpGet("test")]
        public async Task<IActionResult> test()
        {
            var data = _context.OrderWorks.GroupBy(x => new {x.Status,  x.Type })
                .Select(g => new  { Type = g.Key, count = g.Count() }); 
           return Ok(data);
        }


        [HttpGet("GetReasonCatalog/{type}")]
        public async Task<IActionResult> GetReasonCatalog([FromRoute] string type)
        {
            try
            {
                var ReasonCatalog = _context.ReasonCatalog.Where(x => x.Type == type).ToList();
                return Ok(ReasonCatalog);
            }
            catch ( Exception e) {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message});
            }
        }


        [HttpPost("SetReasonCatalogs/{orderWorkId}")]
        public async Task<IActionResult> SetReasonCatalogs([FromRoute] int orderWorkId, [FromBody] List<int> idsReason)
        {
            try
            {
                OrderWorkReasonCatalog OrderWorkReasonCatalog = null;
                idsReason.ForEach(x =>
                {
                    OrderWorkReasonCatalog = new OrderWorkReasonCatalog()
                    {
                        OrderWorkId = orderWorkId,
                        ReasonCatalogId = x
                    
                    };
                    _context.OrderWorkReasonCatalog.Add(OrderWorkReasonCatalog);
                    _context.SaveChanges();

                });

                return Ok("success");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = e.Message });
            }

        }

        [HttpGet("PanelDataStaff")]
        public async Task<IActionResult> GetPanelDataStaff()
        {
            try
            {
                var orderwork = _context.TechnicalStaffs.Include(x => x.OrderWorks).ToList();
                //var orderwork = _context.OrderWorks.Include(x => x.TechnicalStaff).Where(x => x.DateOrder.Date >= Dstart && x.DateOrder.Date <= Dend).ToList();
                return Ok(orderwork);
            }
            catch (Exception error)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "Ocurrió un problema con la petición... " + error });
            }
        }

    }
}