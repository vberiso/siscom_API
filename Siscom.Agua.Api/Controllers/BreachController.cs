using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/Breach/")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class BreachController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;

        public BreachController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }


        // GET: api/Breach
        [HttpGet]
        public IEnumerable<Breach> GetBreach()
        {

           
            return _context.Breaches;

        }

        // GET: api/Breach/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBreaches([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var breach = await _context.Breaches
                                    .Include(t => t.TaxUser)
                                        .ThenInclude(ad => ad.TaxAddresses)
                                    .Include(b => b.BreachDetails)
                                        .ThenInclude(l => l.BreachList)
                                    .FirstOrDefaultAsync(a => a.Id == id);

            if (breach == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontre la infracción" });

            }

            return Ok(breach);
        }

        [HttpGet("SearchFolio/{Folio}")]
        public async Task<IActionResult> GetFolio([FromRoute] string folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fol = await _context.Breaches.Include(t => t.TaxUser)
                                                .ThenInclude(ad => ad.TaxAddresses)
                                                .Include(b => b.BreachDetails)
                                                .ThenInclude(l => l.BreachList)
                                                .FirstOrDefaultAsync(a => a.Folio == folio);
            if(fol == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontre la infracción" });

            }

            return Ok(fol);

        }

        [HttpPost("OrderSale/{id}")]
        public async Task<IActionResult> PostOrderSale([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Validación
            var param = await _context.SystemParameters
                                    .Where(x => x.Name == "DAYS_EXPIRE_ORDER").FirstOrDefaultAsync();

            if (param == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de expiración") });

            }

            var factor = await _context.SystemParameters
                                   .Where(x => x.Name == "FACTOR").FirstOrDefaultAsync();

            if (factor == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta el monto de factor de cálculo ") });

            }


            var breach = await _context.Breaches
                                       .Include(x => x.BreachDetails)
                                        .ThenInclude(y => y.BreachList)
                                       .Where(x => x.Id == id)
                                       .FirstOrDefaultAsync();

            if (breach == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontre la infracción" });

            }


            var orderLast = await _context.OrderSales.Where(o => o.IdOrigin == breach.Id).FirstOrDefaultAsync();

            if (orderLast.Status == "EOS01")
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se puede agregar la infracción por el estado de estatus" });

            }

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    OrderSale order = new OrderSale();

                    //Temporal en lo que se crea trigger de asignacion de folio
                    order.Folio = "F-" + breach.Folio;
                    if (order.Folio == null)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No tiene folio" });

                    }
                    order.DateOrder = DateTime.UtcNow.ToLocalTime();
                    if (order.DateOrder == null)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ingresar la fecha" });

                    }
                    order.Amount = breach.Judge;
                    if (order.Amount == 0)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No tiene monto" });

                    }
                    order.OnAccount = 0;
                    order.Year = (short)DateTime.UtcNow.ToLocalTime().Year;
                    order.Period = 0;
                    order.Type = "OM001";
                    order.Status = "EOS01";
                    order.Observation = breach.Reason;
                    order.ExpirationDate = DateTime.UtcNow.ToLocalTime().Date.AddDays(Convert.ToInt16(param.NumberColumn));
                    var valueDate = order.ExpirationDate.DayOfYear;
                    var valDate = order.DateOrder.DayOfYear;
                    if (valueDate > valDate)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No puedes generar la orden por vigencia vence hasta " + order.ExpirationDate });

                    }
                    if (order.Observation == null)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No tiene observaciones" });

                    }
                    order.IdOrigin = breach.Id;
                    if (order.IdOrigin == 0)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para encontrar la infracción" });

                    }
                    order.TaxUserId = breach.TaxUserId;
                    if (order.TaxUserId == 0)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para encontrar el usuario" });

                    }
                    order.DivisionId = 14;

                    List<OrderSaleDetail> orderSaleDetails = new List<OrderSaleDetail>();
                    breach.BreachDetails.ToList().ForEach(x => {
                        OrderSaleDetail orderSaleDetail = new OrderSaleDetail
                        {
                            Quantity = 1,
                            Unity = "",
                            UnitPrice = factor.NumberColumn,
                            HaveTax = false,
                            Description = x.BreachList.Description,
                            CodeConcept = "",
                            NameConcept = x.BreachList.Description,
                            Amount = x.Amount,
                            OnAccount = 0
                        };
                        orderSaleDetails.Add(orderSaleDetail);
                    });



                    order.OrderSaleDetails = orderSaleDetails;
                    if (order.OrderSaleDetails == null)
                    {
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problema para agregar detalles de orden de cobro" });

                    }
                    _context.OrderSales.Add(order);
                    await _context.SaveChangesAsync();

                    scope.Complete();


                }


            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breach);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para generar order de cobro" });
            }




            return StatusCode((int)TypeError.Code.InternalServerError, new { Success = "Orden de cobro generada" });
        }


        [HttpGet("GetStatus")]
        public IEnumerable<Status> GetStatus()
        {
            return _context.Statuses.Where(d => d.GroupStatusId == 7);
        }

        [HttpGet("GetList")]
        public IEnumerable<BreachList> GetList()
        {
            return _context.BreachLists;

        }

        [HttpGet("Search/{license}")]
        public async Task<IActionResult> GetLicense([FromRoute] string license)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breach = await _context.Breaches.Where(l => l.LicensePlate == license).ToListAsync();

            if (breach.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "No hay infracción" });
            }

            return Ok(breach);
        }





       

 //POST: API/BREACH
        [HttpPost]
        public async Task<IActionResult> PostBreach(int BreachId, [FromBody] Breach breanch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Breach NewBreach = new Breach();

            if ((breanch.BreachDetails != null) && ((breanch != null) && (breanch.TaxUserId == 0)))
            {

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                       

                        var param = await _context.SystemParameters
                                            .Where(x => x.Name == "FACTOR").FirstOrDefaultAsync();
                        if (param == null)
                        {
                            return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de salario minimo") });

                        }
                       
                        var getf = await _context.AssignmentTickets.OrderBy(i => i.Id).Where(f => f.Status == "EFT01" && f.TransitPoliceId == breanch.TransitPoliceId && f.Folio == breanch.AssignmentTicketId).FirstOrDefaultAsync();

                        if (getf == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "no existen folios disponibles" });
                        }

                        if(getf.Folio != breanch.AssignmentTicketId)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El folio es incorrecto,favor de verificar" });
                        }

                        getf.Status = "EFT02";

                        _context.Entry(getf).State = EntityState.Modified;
                        await _context.SaveChangesAsync();


                        //NewBreach.TaxUserId = tax.Id;
                        NewBreach.Car = breanch.Car;
                        if (NewBreach.Car == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar carro" });
                        }
                        NewBreach.Series = breanch.Series;
                        
                        NewBreach.Folio = 'I' + getf.Status;
                        if (NewBreach.Folio == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Problema para ingresar folio" });

                        }
                        NewBreach.CaptureDate = breanch.CaptureDate;
                        if (NewBreach.CaptureDate == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar dia de captura" });
                        }
                        NewBreach.Place = breanch.Place;
                        if (NewBreach.Place == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar lugar" });

                        }
                        NewBreach.Sector = breanch.Sector;
                        if (NewBreach.Sector == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar el sector" });

                        }
                        NewBreach.Zone = breanch.Zone;
                        if (NewBreach.Zone == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar zona" });

                        }
                        NewBreach.TypeCar = breanch.TypeCar;
                        if (NewBreach.TypeCar == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar tipo de automovil" });

                        }
                        NewBreach.Service = breanch.Service;
                        if (NewBreach.Service == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar servicio" });

                        }
                        NewBreach.Color = breanch.Color;
                        if (NewBreach.Color == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar el color" });

                        }
                        NewBreach.Reason = breanch.Reason;
                        if (NewBreach.Reason == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar la razon" });

                        }
                        NewBreach.LicensePlate = breanch.LicensePlate;
                        if (NewBreach.LicensePlate == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar la placa" });

                        }
                        NewBreach.Reason = breanch.Reason;
                        if (NewBreach.Reason == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar razon" });

                        }
                        NewBreach.Judge = breanch.Judge;
                        NewBreach.DateBreach = breanch.DateBreach;
                        if (NewBreach.DateBreach == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar dia de la infracción" });

                        }
                        NewBreach.Status = breanch.Status;
                        if (NewBreach.Status == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar estatus" });

                        }
                        NewBreach.AssignmentTicketId = getf.Id;
                        if (NewBreach.AssignmentTicketId == 0)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Error al asignar folio" });

                        }
                        NewBreach.UserId = breanch.UserId;
                        if (NewBreach.UserId == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Falta ingresar el id del usuario" });

                        }

                        NewBreach.TaxUser = breanch.TaxUser;
                        if (NewBreach.TaxUser == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Faltan datos del contribuyente" });

                        }

                        _context.Breaches.Add(NewBreach);
                        //await _context.SaveChangesAsync();





                        decimal sumBreanch = 0;

                        foreach (var list in breanch.BreachDetails)
                        {
                            var value = await _context.BreachLists.Where(b => b.Id == list.BreachListId).FirstOrDefaultAsync();


                            var getLicense = await _context.Breaches.Where(z => z.LicensePlate.Contains(breanch.LicensePlate)).ToListAsync();
                            int cont = getLicense.Count;


                            if (cont > 1)
                            {
                                var valueJudge = value.MaxTimesFactor;
                                BreachDetail listBreanch = new BreachDetail
                                {
                                    //Breach = NewBreach,
                                    BreachId = NewBreach.Id,

                                    //BreachList = value,
                                    BreachListId = value.Id,
                                    Amount = valueJudge * param.NumberColumn,
                                    Bonification = 0,
                                    PercentBonification = 0,
                                    TimesFactor = valueJudge


                                };

                                sumBreanch += listBreanch.Amount;



                                _context.Add(listBreanch);
                                _context.SaveChanges();

                            }
                            else
                            {
                                var valueJudge = value.MinTimesFactor;
                                BreachDetail listBreanch = new BreachDetail
                                {
                                    //Breach = NewBreach,
                                    BreachId = NewBreach.Id,

                                    //BreachList = value,
                                    BreachListId = value.Id,
                                    Amount = valueJudge * param.NumberColumn,
                                    Bonification = 0,
                                    PercentBonification = 0,
                                    TimesFactor = valueJudge


                                };

                                sumBreanch += listBreanch.Amount;



                                _context.Add(listBreanch);
                                _context.SaveChanges();
                            }
                            //BreachDetail newBreachDetail = new BreachDetail();
                            //if (breanch.LicensePlate == null){
                            //    //newBreachDetail.AplicationDays = 
                            //}

                        }

                        NewBreach.Judge = sumBreanch;
                        //_context.Entry(NewBreach).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        scope.Complete();




                    }


                }
                catch (Exception e)
                {
                    SystemLog systemLog = new SystemLog();
                    systemLog.Description = e.ToMessageAndCompleteStacktrace();
                    systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                    systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                    systemLog.Parameter = JsonConvert.SerializeObject(breanch);
                    CustomSystemLog helper = new CustomSystemLog(_context);
                    helper.AddLog(systemLog);
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para crear la infracción" });

                }
                return CreatedAtAction("GetBreach", new { id = NewBreach.Id }, NewBreach);

            }
            else
            {
                var taxu = await _context.TaxUsers.FindAsync(breanch.TaxUserId);
                var param = await _context.SystemParameters
                                    .Where(x => x.Name == "FACTOR").FirstOrDefaultAsync();
                if (param == null)
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de salario minimo") });

                }
                var getf = await _context.AssignmentTickets.OrderBy(i => i.Id).Where(f => f.Status == "EFT01" && f.TransitPoliceId == breanch.TransitPoliceId && f.Folio == breanch.AssignmentTicketId).FirstOrDefaultAsync();

                if (getf == null)
                {
                    return StatusCode((int)TypeError.Code.Ok, new { Error = "no existen folios disponibles" });
                }

                if (getf.Folio != breanch.AssignmentTicketId)
                {
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El folio es incorrecto,favor de verificar" });

                }


                getf.Status = "EFT02";

                _context.Entry(getf).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        NewBreach.TaxUserId = taxu.Id;
                        if (NewBreach.TaxUserId == 0)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("No se encontro al usuario") });

                        }
                        NewBreach.Car = breanch.Car;
                        if (NewBreach.Car == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el tipo de automovil") });

                        }
                        NewBreach.Series = breanch.Series;
                        if (NewBreach.Series == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar la serie") });

                        }
                        NewBreach.Folio = 'I' + getf.Status;
                        //NewBreach.Folio = getf.Folio;
                        if (NewBreach.Folio == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Problema para agregar folio") });

                        }
                        NewBreach.CaptureDate = breanch.CaptureDate;
                        if (NewBreach.CaptureDate == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el dia captura de la infracción") });

                        }
                        NewBreach.Place = breanch.Place;
                        if (NewBreach.Place == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el lugar") });

                        }
                        NewBreach.Sector = breanch.Sector;
                        if (NewBreach.Sector == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el sector") });

                        }
                        NewBreach.Zone = breanch.Zone;
                        if (NewBreach.Zone == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar la zona") });

                        }
                        NewBreach.TypeCar = breanch.TypeCar;
                        if (NewBreach.TypeCar == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el tipo de automovil") });

                        }
                        NewBreach.Service = breanch.Service;
                        if (NewBreach.Service == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el servicio") });

                        }
                        NewBreach.Color = breanch.Color;
                        if (NewBreach.Color == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el color") });

                        }
                        NewBreach.Reason = breanch.Reason;
                        if (NewBreach.Reason == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar la razon") });

                        }
                        NewBreach.LicensePlate = breanch.LicensePlate;
                        if (NewBreach.LicensePlate == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar la placa") });

                        }
                        NewBreach.Judge = breanch.Judge;
                        NewBreach.DateBreach = breanch.DateBreach;
                        if (NewBreach.DateBreach == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el dia de alta de la infracción") });

                        }
                        NewBreach.Status = breanch.Status;
                        if (NewBreach.Status == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el estatus") });

                        }
                        NewBreach.AssignmentTicketId = getf.Id;
                        if (NewBreach.AssignmentTicketId == 0)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Error para ingresar el folio") });

                        }
                        NewBreach.UserId = breanch.UserId;
                        if (NewBreach.UserId == null)
                        {
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Message = string.Format("Falta ingresar el usuario que crea la infracción") });

                        }


                        _context.Breaches.Add(NewBreach);

                        decimal sumBreanch = 0;

                        foreach (var list in breanch.BreachDetails)
                        {
                            var value = await _context.BreachLists.Where(b => b.Id == list.BreachListId).FirstOrDefaultAsync();
                            var getLicense = await _context.Breaches.Where(z => z.LicensePlate.Contains(breanch.LicensePlate)).ToListAsync();
                            int cont = getLicense.Count;


                            if (cont > 1)
                            {
                                var valueJudge = value.MaxTimesFactor;
                                BreachDetail listBreanch = new BreachDetail
                                {
                                    //Breach = NewBreach,
                                    BreachId = NewBreach.Id,

                                    //BreachList = value,
                                    BreachListId = value.Id,
                                    Amount = valueJudge * param.NumberColumn,
                                    Bonification = 0,
                                    PercentBonification = 0,
                                    TimesFactor = valueJudge


                                };

                                sumBreanch += listBreanch.Amount;



                                _context.Add(listBreanch);
                                _context.SaveChanges();

                            }
                            else
                            {
                                var valueJudge = value.MinTimesFactor;
                                BreachDetail listBreanch = new BreachDetail
                                {
                                    //Breach = NewBreach,
                                    BreachId = NewBreach.Id,

                                    //BreachList = value,
                                    BreachListId = value.Id,
                                    Amount = valueJudge * param.NumberColumn,
                                    Bonification = 0,
                                    PercentBonification = 0,
                                    TimesFactor = valueJudge


                                };

                                sumBreanch += listBreanch.Amount;



                                _context.Add(listBreanch);
                                _context.SaveChanges();
                            }
                        }


                        NewBreach.Judge = sumBreanch;
                        //_context.Entry(NewBreach).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        scope.Complete();

                    }
                }
                catch (Exception e)
                {
                    SystemLog systemLog = new SystemLog();
                    systemLog.Description = e.ToMessageAndCompleteStacktrace();
                    systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                    systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                    systemLog.Parameter = JsonConvert.SerializeObject(breanch);
                    CustomSystemLog helper = new CustomSystemLog(_context);
                    helper.AddLog(systemLog);
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para agregar la infracción" });
                }

                return CreatedAtAction("GetBreach", new { id = NewBreach.Id }, NewBreach);

            }


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBreach([FromRoute] int id, [FromBody] Breach breach)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (id != breach.Id)
                    {
                        return BadRequest();
                    }
                    _context.Entry(breach).State = EntityState.Modified;

                    try
                    {

                        await _context.SaveChangesAsync();


                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BreachExist(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    scope.Complete();
                    return Ok(breach);
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breach);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para editar  infracción" });

            }




        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBreach([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breach = await _context.Breaches.FindAsync(id);
            if (breach == null)
            {
                return NotFound();
            }

            _context.Breaches.Remove(breach);
            await _context.SaveChangesAsync();

            return Ok(breach);
        }


        private bool BreachExist(int id)
        {
            return _context.Breaches.Any(e => e.Id == id);
        }
    }
}
