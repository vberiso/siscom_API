using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    public class OrderSalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderSales
        [HttpGet]
        public IEnumerable<OrderSale> GetOrderSales()
        {
            return _context.OrderSales;
        }

        [HttpGet("FindAllOrdersByDate/{date}")]
        public async Task<IActionResult> GetOrderSales(string date)
        {
            DateTime dateTime = new DateTime();
            DateTime.TryParse(date, out dateTime);
            var orders = _context.OrderSales
                                .Include(x => x.TaxUser)
                                .ThenInclude(user => user.TaxAddresses)
                                .Include(x => x.OrderSaleDetails)
                                .Where(x => x.DateOrder.Date == dateTime.Date).ToList();
            orders.ForEach(x =>
            {
                x.DescriptionStatus = (from d in _context.Statuses
                                       where d.CodeName == x.Status && d.GroupStatusId == 10
                                       select d).FirstOrDefault().Description;

                x.DescriptionType = (from d in _context.Types
                                       where d.CodeName == x.Type
                                       select d).FirstOrDefault().Description;
            });
            if (orders.Count > 0)
                return Ok(orders);
            else
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "La fecha que ingreso no contiene datos favor de verificar" });
        }

        [HttpGet("FindAllOrdersByDateBreach/{date}")]
        public async Task<IActionResult> GetOrderSalesBreach(string date)
        {
            DateTime dateTime = new DateTime();
            DateTime.TryParse(date, out dateTime);
            var orders = _context.OrderSales
                                .Include(x => x.TaxUser)
                                .ThenInclude(user => user.TaxAddresses)
                                .Include(x => x.OrderSaleDetails)
                                .Where(x => x.DateOrder.Date == dateTime.Date && x.Type == "OM001").ToList();
            orders.ForEach(x =>
            {
                x.DescriptionStatus = (from d in _context.Statuses
                                       where d.CodeName == x.Status && d.GroupStatusId == 10
                                       select d).FirstOrDefault().Description;

                x.DescriptionType = (from d in _context.Types
                                     where d.CodeName == x.Type 
                                     select d).FirstOrDefault().Description;
            });
            if (orders.Count > 0)
                return Ok(orders);
            else
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "La fecha que ingreso no contiene datos favor de verificar" });
        }


        [HttpGet("FindAllOrderTaxUser/{userId}")]
        public async Task<IActionResult> GetOrderSalesBreach(int userId)
        {
           
            var orders = _context.OrderSales
                                .Include(x => x.TaxUser)
                                .ThenInclude(user => user.TaxAddresses)
                                .Include(x => x.OrderSaleDetails)
                                .Where(x => x.TaxUserId == userId).ToList();
           
            if (orders.Count > 0)
                return Ok(orders);
            else
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No tiene ordenes asociadas" });
        }

        //[HttpGet("FindAllOrdersByAccount/{account}")]
        //public async Task<IActionResult> GetOrderSalesByAccount([FromRoute]string account)
        //{
        //    var orders = _context.OrderSales
        //                        .Include(x => x.TaxUser)
        //                        .ThenInclude(user => user.TaxAddresses)
        //                        .Include(x => x.OrderSaleDetails)
        //                        .Where(x => x.DateOrder.Date == dateTime.Date).ToList();
        //    orders.ForEach(x =>
        //    {
        //        x.DescriptionStatus = (from d in _context.Statuses
        //                               where d.CodeName == x.Status && d.GroupStatusId == 10
        //                               select d).FirstOrDefault().Description;

        //        x.DescriptionType = (from d in _context.Types
        //                             where d.CodeName == x.Type
        //                             select d).FirstOrDefault().Description;
        //    });
        //    if (orders.Count > 0)
        //        return Ok(orders);
        //    else
        //        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "La fecha que ingreso no contiene datos favor de verificar" });
        //}

        // GET: api/OrderSales/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderSale([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderSale = await _context.OrderSales
                                          .Include(x => x.TaxUser)
                                            .ThenInclude(user => user.TaxAddresses)
                                          .Include(x => x.OrderSaleDetails)
                                          .Where(x => x.Id == id)
                                          .FirstOrDefaultAsync();

            if (orderSale == null)
            {
                return NotFound();
            }

            return Ok(orderSale);
        }

        // GET: api/OrderSales/5
        [HttpGet("Folio/{folio}")]
        [Authorize]
        public async Task<IActionResult> GetOrderSaleFolio([FromRoute] string folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(folio))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Folio incorrecto" });

            var orderSale = await _context.OrderSales
                                          .Include(x => x.TaxUser)
                                            .ThenInclude(y => y.TaxAddresses)
                                          .Include(x => x.OrderSaleDetails)
                                          .Where(x => x.Folio == folio && (x.Status != "EOS02" && x.Status != "EOS03"))
                                          .FirstOrDefaultAsync();

            if (orderSale == null)
            {
                return NotFound();
            }

            orderSale.DescriptionStatus = await _context.Statuses
                                                        .Where(x => x.CodeName == orderSale.Status)
                                                        .Select(x => x.Description)
                                                        .FirstOrDefaultAsync();

            orderSale.DescriptionType = await _context.Types
                                                        .Where(x => x.CodeName == orderSale.Type)
                                                        .Select(x => x.Description)
                                                        .FirstOrDefaultAsync();

            return Ok(orderSale);
        }

        // PUT: api/OrderSales/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutOrderSale([FromRoute] int id, [FromBody] OrderSale orderSale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderSale.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderSale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderSaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/OrderSales
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostOrderSale([FromBody] OrderSale orderSale)
        {

            OrderSale _orderSale = new OrderSale();
            decimal totalIva = 0;
            decimal total = 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            #region Validación
            var param = await _context.SystemParameters
                                    .Where(x => x.Name == "DAYS_EXPIRE_ORDER").FirstOrDefaultAsync();

            if (param == null)
                return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de expiración") });

            decimal _sumTot = orderSale.OrderSaleDetails.Sum(x => (x.Amount + x.Tax));
            decimal _sumIVA = orderSale.OrderSaleDetails.Sum(x => x.Tax);
            decimal IVA = _context.SystemParameters.Where(x => x.Name == "IVA" && x.IsActive == true).FirstOrDefault().NumberColumn;

            orderSale.OrderSaleDetails.ToList().ForEach(x =>
            {
                var product = _context.TariffProducts
                                     .Include(y => y.Product)
                                       .ThenInclude(prod=> prod.ProductParams)
                                     .Where(y => y.ProductId == Convert.ToInt32(x.CodeConcept) &&
                                                  y.IsActive == 1).SingleOrDefault();
                if(product.Type == "TTP01")
                {
                    if (product.HaveTax)
                    {
                        totalIva += Math.Round((((product.Amount * x.Quantity) * IVA) / 100), 2);
                    }
                    total += x.Amount;
                }
                else
                {
                    if (product.HaveTax)
                    {
                        totalIva += Math.Round(((x.Amount * IVA) / 100), 2);
                    }
                    total += x.Amount;
                }
                
            });
            if(_sumIVA != totalIva)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El IVA calculado no es correcto en su detalle ") });
            }
            if (_sumTot != (totalIva + total))
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos a pagar en el detalle no son correctos") });
            }
            #endregion


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    TaxUser _taxUser = new TaxUser();

                    if (orderSale.TaxUserId != 0)
                    {
                        _taxUser = await _context.TaxUsers.FindAsync(orderSale.TaxUserId);
                        if (_taxUser == null)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No existe cliente proporcionado" });
                    }
                    else
                    {
                        //if (orderSale.TaxUserId == 0)
                        //    _taxUser.Id = 16;
                        //else
                        //{

                        if (string.IsNullOrEmpty(orderSale.TaxUser.Name))
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Debe proporcionar nombre de cliente" });

                        _taxUser = orderSale.TaxUser;
                        _taxUser.IsActive = true;
                        _context.TaxUsers.Add(_taxUser);
                        //}
                    }

                    var paramSystem = await _context.SystemParameters.Where(x => x.Name == "ISMUNICIPAL" && x.IsActive == true).FirstOrDefaultAsync();
                    if (paramSystem == null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Falta parametro de configuración" });

                    _orderSale.DateOrder = DateTime.UtcNow.ToLocalTime();
                    _orderSale.Amount = orderSale.Amount;
                    _orderSale.OnAccount = 0;
                    _orderSale.Year = Convert.ToInt16(DateTime.UtcNow.ToLocalTime().Year);
                    _orderSale.Period = 1;
                    _orderSale.Type = orderSale.Type;
                    _orderSale.Status = "EOS01";
                    _orderSale.Observation = orderSale.Observation;
                    _orderSale.IdOrigin = orderSale.IdOrigin;
                    _orderSale.TaxUserId = _taxUser.Id;
                    _orderSale.ExpirationDate = DateTime.UtcNow.ToLocalTime().Date.AddDays(Convert.ToInt16(param.NumberColumn));
                    _orderSale.DivisionId = orderSale.DivisionId == 0 ? (paramSystem.TextColumn == "NO" ? 1 : 15) : orderSale.DivisionId;
                    _orderSale.OrderSaleDetails = orderSale.OrderSaleDetails;

                    _context.OrderSales.Add(_orderSale);

                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "ProductController";
                systemLog.Action = "PostProductAgreement";
                systemLog.Parameter = JsonConvert.SerializeObject(orderSale);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            RedirectToActionResult redirect = new RedirectToActionResult("GetOrderSaleById", "OrderSales", new { @id = _orderSale.Id });
            return redirect;
            //return Ok(_orderSale);
            //return RedirectToAction("GetOrderSaleById", "OrderSales", new { id = _orderSale.Id });
        }

        [HttpGet("GetOrderSaleById", Name = "GetOrderSaleById")]
        public async Task<IActionResult> GetOrderSaleById(int id)
        {

            var orderSale = await _context.OrderSales
                                          .Include(x => x.OrderSaleDetails)
                                          .Include(x => x.TaxUser)
                                            .ThenInclude(user => user.TaxAddresses)
                                          .Where(x => x.Id == id)
                                          .FirstOrDefaultAsync();

            if (orderSale == null)
            {
                return NotFound();
            }

            return Ok(orderSale);
        }


        // POST: api/OrderSales
        [HttpPost("Edit")]
        [Authorize]
        public async Task<IActionResult> PostOrderSaleEdit([FromBody] OrderSale orderSale)
        {

            OrderSale _orderSale = new OrderSale();
            decimal totalIva = 0;
            decimal total = 0;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            #region Validación
            var param = await _context.SystemParameters
                                    .Where(x => x.Name == "DAYS_EXPIRE_ORDER").FirstOrDefaultAsync();

            if (param == null)
                return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de expiración") });

            decimal _sumTot = orderSale.OrderSaleDetails.Sum(x => (x.Amount + x.Tax));
            decimal _sumIVA = orderSale.OrderSaleDetails.Sum(x => x.Tax);
            decimal IVA = _context.SystemParameters.Where(x => x.Name == "IVA" && x.IsActive == true).FirstOrDefault().NumberColumn;

            orderSale.OrderSaleDetails.ToList().ForEach(x =>
            {
                var product = _context.TariffProducts
                                     .Include(y => y.Product)
                                       .ThenInclude(prod => prod.ProductParams)
                                     .Where(y => y.ProductId == Convert.ToInt32(x.CodeConcept) &&
                                                  y.IsActive == 1).SingleOrDefault();
                if (product.Type == "TTP01")
                {
                    if (product.HaveTax)
                    {
                        totalIva += Math.Round((((product.Amount * x.Quantity) * IVA) / 100), 2);
                    }
                    total += x.Amount;
                }
                else
                {
                    if (product.HaveTax)
                    {
                        totalIva += Math.Round(((x.Amount * IVA) / 100), 2);
                    }
                    total += x.Amount;
                }

            });
            if (_sumIVA != totalIva)
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El IVA calculado no es correcto en su detalle ") });
            }
            if (_sumTot != (totalIva + total))
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos a pagar en el detalle no son correctos") });
            }
            #endregion


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    TaxUser _taxUser = new TaxUser();

                    if (orderSale.TaxUserId != 0)
                    {
                        _taxUser = await _context.TaxUsers.FindAsync(orderSale.TaxUserId);
                        if (_taxUser == null)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No existe cliente proporcionado" });
                    }
                    else
                    {                        
                        if (string.IsNullOrEmpty(orderSale.TaxUser.Name))
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Debe proporcionar nombre de cliente" });

                        _taxUser = orderSale.TaxUser;
                        _taxUser.IsActive = true;
                        _context.TaxUsers.Add(_taxUser);
                       
                    }

                    var paramSystem = await _context.SystemParameters.Where(x => x.Name == "ISMUNICIPAL" && x.IsActive == true).FirstOrDefaultAsync();
                    if (paramSystem == null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Falta parametro de configuración" });

                    _orderSale.DateOrder = DateTime.UtcNow.ToLocalTime();
                    _orderSale.Amount = orderSale.Amount;
                    _orderSale.OnAccount = 0;
                    _orderSale.Year = Convert.ToInt16(DateTime.UtcNow.ToLocalTime().Year);
                    _orderSale.Period = 1;
                    _orderSale.Type = orderSale.Type;
                    _orderSale.Status = "EOS01";
                    _orderSale.Observation = orderSale.Observation;
                    _orderSale.IdOrigin = orderSale.IdOrigin;
                    _orderSale.TaxUserId = _taxUser.Id;
                    _orderSale.ExpirationDate = DateTime.UtcNow.ToLocalTime().Date.AddDays(Convert.ToInt16(param.NumberColumn));
                    _orderSale.DivisionId = orderSale.DivisionId == 0 ? (paramSystem.TextColumn == "NO" ? 1 : 15) : orderSale.DivisionId;
                    _orderSale.OrderSaleDetails = orderSale.OrderSaleDetails;

                    _context.OrderSales.Add(_orderSale);

                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "ProductController";
                systemLog.Action = "PostProductAgreement";
                systemLog.Parameter = JsonConvert.SerializeObject(orderSale);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            RedirectToActionResult redirect = new RedirectToActionResult("GetOrderSaleById", "OrderSales", new { @id = _orderSale.Id });
            return redirect;            
        }

        // DELETE: api/OrderSales/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrderSale([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderSale = await _context.OrderSales.FindAsync(id);
            if (orderSale == null)
            {
                return NotFound();
            }

            _context.OrderSales.Remove(orderSale);
            await _context.SaveChangesAsync();

            return Ok(orderSale);
        }

        private bool OrderSaleExists(int id)
        {
            return _context.OrderSales.Any(e => e.Id == id);
        }
    }
}