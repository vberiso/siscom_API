
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
    [ApiController]
    [Authorize]
    public class OrderWorkController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private List<string> statusDeuda = new List<string>() { "ED001", "ED004", "ED007", "ED011" };
        public OrderWorkController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("OrderWorks/{id?}")]
        public async Task<IActionResult> GetOrderWorks([FromQuery] string id = null, [FromQuery] string status = null, [FromQuery] string folio = null, [FromQuery] string type = null, [FromQuery] string fecha = null, [FromQuery] string IsMasivo = null, [FromQuery] bool withOrder = false, [FromQuery] string account= null)
        {
            if (id == null)
            {
                System.Linq.IQueryable<OrderWork> query = _context.OrderWorks.Include(x => x.Agreement);
                
                if (folio == null)
                {
                  
                    if (account != null)
                    {
                        query = query.Where(x => x.Agreement.Account == account);
                    }
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
                    if (IsMasivo != null)
                    {
                        query = query.Where(x => x.Status == "EOT01");
                    }
                }
                else
                {
                    query = query.Where(x => x.Folio == folio);
                }


                query = query.OrderBy(x => x.Folio);

                var OrderWorks = query.ToList();

                return Ok(OrderWorks);
            }
            try
            {

                var OrderWork = _context.OrderWorks.Where(x => x.Id.ToString() == id)
                    .Include(x => x.OrderWorkReasonCatalogs)
                    .ThenInclude(x => x.ReasonCatalog)
                    .Include(x => x.PhotosOrderWork)
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
            List<object> agreements = new List<object>();
            try
            {
               

                foreach (var item in list)
                {
                    var OrderWork = _context.OrderWorks.Where(x => x.Id == item).Include(x => x.TechnicalStaff).First();
                    var agreement = _context.Agreements.Where(a => a.Id == OrderWork.AgrementId)
                        .Include(x => x.TypeIntake)
                        .Include(x => x.Debts)
                            .ThenInclude(x => x.DebtDetails)
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
                    agreement.Debts = agreement.Debts.Where(d =>  statusDeuda.Contains(d.Status)).ToList();
                    agreement.OrderWork = new List<OrderWork>() { OrderWork };


                    agreements.Add(JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(agreement, Formatting.Indented,
                           new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           })));
                  
                }


                return Ok(agreements);
            }
            catch (Exception ex)
            {
                return Ok("");
            }
        }

        [HttpGet("FromAccount/{account}")]
        public async Task<IActionResult> GetFromAccount([FromRoute] string account)
        {            
            try
            {  
                var agreement = _context.Agreements.Where(a => a.Account == account)
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

                if (agreement == null)
                {
                    return NotFound();
                }

                return Ok(agreement);
            }
            catch (Exception ex)
            {
                return BadRequest("error:" + ex.Message);
            }
        }

        private string ValidationOrderWorkAviso(Agreement agreement, bool isAyuntamiento)
        {
       
            agreement.Debts = agreement.Debts.Where(x => statusDeuda.Contains(x.Status)).ToList();
            if (agreement.Debts.Count > 0) {
                DateTime maxPeriod = agreement.Debts.Max(x => x.UntilDate);
                DateTime minPeriod = agreement.Debts.Min(x => x.FromDate);
                DateTime toDay = DateTime.Now;
                var notificaciones = _context.Notifications.Where(x => x.AgreementId == agreement.Id && (x.NotificationDate >= minPeriod && x.NotificationDate <= maxPeriod)).ToList();
                if (notificaciones.Count < 2)
                {
                    return " 1, debe tener minimo dos notificaciones";
                }

                var orders = agreement.OrderWork.Where(x => x.Type == "OT006" && (x.DateOrder >= minPeriod && x.DateOrder <= maxPeriod)).ToList();
                if (orders.Where(x => x.Status == "EOT01" || x.Status == "EOT02").ToList().Count == 1)
                {
                    return ", porque tiene una orden de aviso de adeudo en proceso";
                }
                int numOrders = orders.Where(x => x.Status == "EOT03").ToList().Count;
                return numOrders == 0 ? "1" : (numOrders == 1 ? "2" : ", porque ya tiene dos ordenes de aviso de adeudo.");


            }
            return ", debe tener deuda";
        }
        [HttpPost("OrderWorks/{isAyuntamiento?}")]
        public async Task<IActionResult> Create([FromBody] object collection ,[FromRoute] bool isAyuntamiento = false)
        {
            int ApplyIds = 0;
            var data = JObject.Parse(collection.ToString());
            try
            {
                string[] ids = JsonConvert.DeserializeObject<string[]>(data["ids"].ToString());

                List<string> msgs = new List<string>();
                OrderWork OrderWork = null;
                Agreement Aggrement;
                bool canCreate;
                string avisoError = "";
                int aviso = 0 ;
                foreach (string id in ids)
                {
                    canCreate = true;
                  
                    
                    if (data["typeOrder"].ToString() == "OT003" || data["typeOrder"].ToString() == "OT006")
                    {
                        Aggrement = _context.Agreements.Where(x => x.Id == int.Parse(id))
                            .Include(x => x.Clients)
                            .Include(x => x.OrderWork)
                            .Include(x => x.Debts)
                            .First();

                        if (data["typeOrder"].ToString() == "OT006")
                        {
                            
                            avisoError = ValidationOrderWorkAviso(Aggrement, isAyuntamiento);
                            bool isNumeric = int.TryParse(avisoError, out aviso);


                            canCreate = isNumeric;
                               
                            
                        }
                        else
                        {
                            OrderWork = Aggrement.OrderWork.Where(x => x.Type == "OT003" && (x.Status == "EOT02" || x.Status == "EOT01")).FirstOrDefault();
                            var deb = Aggrement.Debts.Where(x => x.Status == "ED001" || x.Status == "ED011" || x.Status == "ED007" || x.Status == "ED004").ToList();
                            if (OrderWork != null || deb.Count() > 0 || Aggrement.TypeStateServiceId != 3)
                            {
                                canCreate = false;
                            }
                        }

                        //else if (deb.Count() > 0)
                        //{
                        //    canCreate = false;
                        //}

                        if (!canCreate)
                        {
                            var client = Aggrement.Clients.Where(x => x.TypeUser == "CLI01").First();
                            string tipeOrder = "";
                            switch (data["typeOrder"].ToString())
                            {
                                case "OT001":
                                    tipeOrder = "Inspeccion / Verificacion";
                                    break;
                                case "OT002":
                                    tipeOrder = "Corte";
                                    break;
                                case "OT003":
                                    tipeOrder = "Reconexion";
                                    break;
                                case "OT004":
                                    tipeOrder = "Mantenimiento / Sustitucion";
                                    break;
                                case "OT006":
                                    tipeOrder = "Aviso de deuda";
                                     
                                    break;
                            }
                            msgs.Add($"La cuenta {Aggrement.Account} con nombre de cliente {client.Name} {client.LastName} no se pudo generar una orden de tipo {tipeOrder}{avisoError}");
                            continue;
                        }
                    }
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
                        Activities = data["Activities"].ToString(),
                        aviso = aviso


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
                if (msgs.Count > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { reazon = msgs, msg = "Algunas ordenes no se pudieron generar", id = "" });
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
                int id_agreement = 0; ;
                if (OrderWork.Agreement != null)
                {
                     id_agreement = OrderWork.Agreement.Id;
                    OrderWork.Agreement = null;
                }
                
                
                _context.OrderWorks.Update(OrderWork);
                //_context.Entry(OrderWork).State = EntityState.Modified;
                _context.SaveChanges();
                if ((OrderWork.Type == "OT002" || OrderWork.Type == "OT003") && OrderWork.Status == "EOT03" && id_agreement!=0)
                {
                    var agreement = _context.Agreements.Where(x => x.Id == id_agreement).First();
                    agreement.TypeStateServiceId = OrderWork.Type == "OT002" ? 3 : 1;

                    _context.Agreements.Update(agreement);
                    _context.SaveChanges();
                }
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
            var data = _context.OrderWorks.GroupBy(x => new { x.Status, x.Type })
                .Select(g => new { Type = g.Key, count = g.Count() });
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = e.Message });
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

        [HttpGet("PanelData/{dateStart}/{dateEnd}")]
        public async Task<IActionResult> GetPanelData([FromRoute] string dateStart, [FromRoute] string dateEnd)
        {
            DateTime Dstart = new DateTime();
            DateTime DEnd = new DateTime();
            DateTime.TryParse(dateStart, out Dstart);
            DateTime.TryParse(dateEnd, out DEnd);
            try
            {
                var generadas = _context.OrderWorks.Where(g => g.DateOrder >= Dstart && g.DateOrder <= DEnd.AddDays(1) && g.Status == "EOT01").ToListAsync();
                var asignadas = _context.OrderWorks.Where(g => g.DateOrder >= Dstart && g.DateOrder <= DEnd.AddDays(1) && g.Status == "EOT02").ToListAsync();
                var ejecutadas = _context.OrderWorks.Where(g => g.DateOrder >= Dstart && g.DateOrder <= DEnd.AddDays(1) && g.Status == "EOT03").ToListAsync();
                var noEjecutada = _context.OrderWorks.Where(g => g.DateOrder >= Dstart && g.DateOrder <= DEnd.AddDays(1) && g.Status == "EOT04").ToListAsync();

                var list = await Task.WhenAll(generadas, asignadas, ejecutadas, noEjecutada);
                if (list[0].Count == 0 && list[1].Count == 0 && list[2].Count == 0 && list[3].Count == 0)
                {
                    return StatusCode((int)TypeError.Code.NotFound, new { Error = "No se encontraron datos en el rango de fecha especificado, posiblemente es una fecha que aun no ha llegado o no hay registros actualmente" });
                } else
                {
                    return Ok(list);
                }
                
            }
            catch (Exception error)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "Ocurrió un problema con la petición... " + error });
            }
        }

        [HttpGet("PanelDataStaff")]
        public async Task<IActionResult> GetPanelDataStaff()
        {
            try
            {
                var orderwork = _context.TechnicalStaffs.Include(x => x.OrderWorks).ToList();
                return Ok(orderwork);
            }
            catch (Exception error)
            {
                return StatusCode((int)TypeError.Code.NotFound, new { Error = "Ocurrió un problema con la petición... " + error });
            }
        }
        [HttpPost("uploadFile")]
        public async Task<IActionResult> UoloadFile([FromBody] List<PhotosOrderWork> Photos)
        {
            try
            {
                foreach (var Photo in Photos)
                {
                    _context.Add(Photo);
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}