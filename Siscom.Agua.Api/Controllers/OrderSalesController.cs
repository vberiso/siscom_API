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


        [HttpGet("FindAllOrderTaxUser/{userId}/{folio}")]
        public async Task<IActionResult> GetOrderSalesTaxUser(int userId,string folio)
        {
           
            var orders = _context.OrderSales
                                .Include(x => x.TaxUser)
                                .ThenInclude(user => user.TaxAddresses)
                                .Include(x => x.OrderSaleDetails)
                                .Where(x => x.TaxUserId == userId && x.Folio != folio ).ToList();
           
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
        public async Task<IActionResult> GetOrderSaleFolio([FromRoute] string folio, bool soloPagadas=true)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(folio))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Folio incorrecto" });

            //Valida si es una infracción
            if (char.IsLetter(Convert.ToChar(folio.Substring(0, 1))) && folio.Contains("-") && folio.Substring(0, 1).Equals("I"))
                ValidaDescuentoDeInfraccion(folio);


            var query = _context.OrderSales
                                        .Include(x => x.TaxUser)
                                        .ThenInclude(y => y.TaxAddresses)
                                        .Include(x => x.OrderSaleDetails);
            Siscom.Agua.DAL.Models.OrderSale orderSale;
            if (soloPagadas)
                 orderSale = await query.Where(x => x.Folio == folio && (x.Status != "EOS02" && x.Status != "EOS03"))
                                          .FirstOrDefaultAsync();
            else
                 orderSale = await query.Where(x => x.Folio == folio)
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

        //Valida el descuento correspondiente para la infraccion, segun su fecha de emision.
        private void ValidaDescuentoDeInfraccion(string folio)
        {
            try
            {
                var OS = _context.OrderSales                                        
                                        .Include(x => x.OrderSaleDetails)
                                        .Include(x => x.OrderSaleDiscounts)
                                        .Where(x => x.Folio == folio && x.Status == "EOS01")
                                        .FirstOrDefault();

                var lstCamp = _context.DiscountCampaigns.Where(c => c.Name.Contains("INFDES") && c.IsActive == true).OrderBy(x => x.Id).ToList();
                // Obtengo la fecha que se genera la infraccion.
                var B = _context.Breaches
                            .Where(b => b.Id == OS.IdOrigin).FirstOrDefault();

                var dias = (DateTime.Today - B.DateBreach).TotalDays;
                foreach (var item in lstCamp)
                {
                    int DiasVigencia = int.Parse(item.Name.Split("_")[1]);
                    if (dias <= DiasVigencia)
                    {
                        if (OS.OrderSaleDiscounts == null || OS.OrderSaleDiscounts.Count == 0)     //Si aun no existe ningun descuento
                        {
                            List<OrderSaleDiscount> lstOSDis = new List<OrderSaleDiscount>();
                            foreach (var OSD in OS.OrderSaleDetails.ToList())
                            {
                                OrderSaleDiscount OSDis = new OrderSaleDiscount();
                                OSDis.CodeConcept = OSD.CodeConcept;
                                OSDis.NameConcept = OSD.NameConcept;
                                OSDis.OriginalAmount = OSD.Amount;
                                OSDis.DiscountAmount = decimal.Round(OSD.Amount * ((decimal)item.Percentage / 100), 2);
                                OSDis.DiscountPercentage = item.Percentage;
                                OSDis.OrderSaleId = OS.Id;
                                OSDis.OrderSaleDetailId = OSD.Id;
                                lstOSDis.Add(OSDis);

                                OSD.Amount = OSD.Amount - OSDis.DiscountAmount;
                            }

                            OS.OrderSaleDiscounts = lstOSDis;
                            OS.Amount = OS.OrderSaleDetails.Sum(osd => osd.Amount);

                            if (OS.Observation.Contains(", Descuento a infracción del"))
                            {
                                string textoAQuitar = OS.Observation.Substring(OS.Observation.IndexOf(", Descuento a infracción del"), 32);
                                OS.Observation = OS.Observation.Replace(textoAQuitar, "");
                            }
                            OS.Observation = OS.Observation.TrimEnd();
                            OS.Observation += ", Descuento a infracción del " + item.Percentage + "% ";
                        }
                        else       //Si se esta editando los descuento.
                        {
                            foreach (var OSDis in OS.OrderSaleDiscounts.ToList())
                            {
                                OSDis.DiscountAmount = decimal.Round(OSDis.OriginalAmount * ((decimal)item.Percentage / 100), 2);
                                OSDis.DiscountPercentage = item.Percentage;

                                OS.OrderSaleDetails.Where(x => x.Id == OSDis.OrderSaleDetailId).FirstOrDefault().Amount = OSDis.OriginalAmount - OSDis.DiscountAmount;
                            }
                            OS.Amount = OS.OrderSaleDetails.Sum(osd => osd.Amount);

                            if (OS.Observation.Contains(", Descuento a infracción del"))
                            {
                                string textoAQuitar = OS.Observation.Substring(OS.Observation.IndexOf(", Descuento a infracción del"), 32);
                                OS.Observation = OS.Observation.Replace(textoAQuitar, "");
                            }
                            OS.Observation = OS.Observation.TrimEnd();
                            OS.Observation += ", Descuento a infracción del " + item.Percentage + "% ";
                        }

                        _context.Entry(OS).State = EntityState.Modified;
                        _context.SaveChanges();
                        return;
                    }
                    else
                    {
                        dias = dias - DiasVigencia;
                    }
                }
                //Si llega a esta etapa significa que agoto los dias de tolerancia para descuentos.
                if (dias > 0)
                {
                    if (OS.OrderSaleDiscounts == null || OS.OrderSaleDiscounts.Count == 0)     //Si aun no existe ningun descuento
                    {
                        //No sucede nada, la deuda debe permanecer igual.
                    }
                    else       //Si se esta editando los descuento.
                    {
                        foreach (var OSDis in OS.OrderSaleDiscounts.ToList())
                        {
                            OSDis.DiscountAmount = 0;
                            OSDis.DiscountPercentage = 0;
                            OS.OrderSaleDetails.Where(x => x.Id == OSDis.OrderSaleDetailId).FirstOrDefault().Amount = OSDis.OriginalAmount;
                        }
                        OS.Amount = OS.OrderSaleDetails.Sum(osd => osd.Amount);
                        if (OS.Observation.Contains(", Descuento a infracción del"))
                        {
                            string textoAQuitar = OS.Observation.Substring(OS.Observation.IndexOf(", Descuento a infracción del"), 32);
                            OS.Observation = OS.Observation.Replace(textoAQuitar, "");
                            OS.Observation = OS.Observation.TrimEnd();
                        }
                    }
                    _context.Entry(OS).State = EntityState.Modified;
                    _context.SaveChanges();
                    return;
                }
            }
            catch(Exception ex)
            {

            }
        }

        //Obtiene informacion de infracción        
        [HttpGet("FolioInfraccion/{folio}")]
        [Authorize]
        public async Task<IActionResult> GetOrderSaleFolioInfraccion([FromRoute] string folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(folio))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Folio incorrecto" });

            var orderSale = _context.OrderSales
                                          .Include(x => x.TaxUser)
                                            .ThenInclude(y => y.TaxAddresses)
                                          .Include(x => x.OrderSaleDetails)
                                          .Include(dis => dis.OrderSaleDiscounts)
                                          .Where(os => os.Status == "EOS01")
                                          .FirstOrDefault();
            if (orderSale == null)            
                return NotFound();
            

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

        [HttpGet("All/Folio/{folio}")]
        [Authorize]
        public async Task<IActionResult> GetOrderSaleByFolio([FromRoute] string folio)
        {
            return await this.GetOrderSaleFolio(folio, false);
        }

        [HttpGet("RFC/{rfc}")]
        [Authorize]
        public async Task<IActionResult> GetOrderSaleRFC([FromRoute] string rfc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           

            var orderSale = await _context.TaxUsers
                                          .Include(x => x.TaxAddresses)
                                          .Where(x => x.RFC == rfc)
                                          .FirstOrDefaultAsync();

            if (orderSale == null)
            {
                return NotFound();
            }

          
            return Ok(orderSale);
        }


        [HttpGet("Name/{name}/{val}")]
        [Authorize]
        public IActionResult GetOrderSalesName([FromRoute] string name, int val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (val == 1)
            {
                var orderSale = _context.TaxUsers
                                     .Include(x => x.TaxAddresses)
                                     .Include(z => z.OrderSales)
                                     .Where(x => x.Name == name)
                                     .ToList();

                if (orderSale == null)
                {
                    return NotFound();
                }


                return Ok(orderSale);
            }
            else
            {
                var orderSale = _context.TaxUsers
                                   .Include(x => x.TaxAddresses)
                                   .Include(z => z.OrderSales)
                                   .Where(x => x.Name == name && x.IsProvider == true)
                                   .ToList();

                if (orderSale == null)
                {
                    return NotFound();
                }


                return Ok(orderSale);
            }



        }

        [HttpGet("RFC/{rfc}/{val}")]
        [Authorize]
        public IActionResult GetOrderSalesRFC([FromRoute] string rfc, int val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (val == 1)
            {
                var orderSale = _context.TaxUsers
                                     .Include(x => x.TaxAddresses)
                                     .Include(z => z.OrderSales)
                                     .Where(x => x.RFC == rfc)
                                     .ToList();

                if (orderSale == null)
                {
                    return NotFound();
                }


                return Ok(orderSale);
            }
            else
            {
                var orderSale = _context.TaxUsers
                                   .Include(x => x.TaxAddresses)
                                   .Include(z => z.OrderSales)
                                   .Where(x => x.RFC == rfc && x.IsProvider == true)
                                   .ToList();

                if (orderSale == null)
                {
                    return NotFound();
                }


                return Ok(orderSale);
            }



        }


        [HttpGet("OrderTaxUser/{idTaxUser}/")]
        [Authorize]
        public IActionResult GetOrderSalesTaxUser ([FromRoute] int idTaxUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


           
                var orderSale = _context.OrderSales
                                     .Include(x => x.OrderSaleDetails)
                                     .Include(z => z.TaxUser)
                                     .Where(x => x.TaxUserId == idTaxUser)
                                     .ToList();

                if (orderSale == null)
                {
                    return NotFound();
                }


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

            var number = 20;



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

                    if (orderSale.DivisionId == 2)
                    {
                        _orderSale.ExpirationDate = DateTime.UtcNow.ToLocalTime().Date.AddDays(number);
                    }
                    else
                    {
                        _orderSale.ExpirationDate = DateTime.UtcNow.ToLocalTime().Date.AddDays(Convert.ToInt16(param.NumberColumn));

                    }
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