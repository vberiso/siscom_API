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
                                            .ThenInclude(s => s.BreachArticle)
                                    .Include(w => w.BreachWarranties)
                                        
                                    .FirstOrDefaultAsync(a => a.Id == id);

            if (breach == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontre la infracción" });

            }

            return Ok(breach);
        }


        [HttpGet("getCarModels")]
        public async Task<IActionResult> GetModelsCar()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var breach =  _context.BrandModels.ToArray();



            if (breach == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No hay "});

            }

            return Ok(breach);
        }


        [HttpGet("getCar/{model}")]
        public async Task<IActionResult> GetCar([FromRoute] string model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var mol = _context.BrandModels.Where(x =>x.Brand == model).ToArray();



            if (mol == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No hay modelos " });

            }

            return Ok(mol);
        }






        [HttpGet("SearchFolio/{Folio}")]
        public async Task<IActionResult> GetFolio([FromRoute] string Folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fol = await _context.Breaches.Include(t => t.TaxUser)
                                                .ThenInclude(ad => ad.TaxAddresses)
                                                .Include(b => b.BreachDetails)
                                                .ThenInclude(l => l.BreachList)
                                                .FirstOrDefaultAsync(a => a.Folio == Folio);
            if(fol == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontro la infracción" });

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

           

            if(breach.Status == "INF04")
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "La infracción ha sido pagada" });

            }

            breach.Status = "INF03";


            //var ser = await _context.Services.ToListAsync();


            var serv =  _context.Services.FirstOrDefault(x => x.Name == "INFRACCIONES");

            if(serv == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontre valor para infracción" });

            }

            var serParam = await _context.SystemParameters.FirstOrDefaultAsync(p => p.Name == "Sanciones Vialidad");

            if(serParam == null)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se encontre valor para infracción " });

            }

            var ticket = await _context.AssignmentTickets
                                                .Include(s => s.TransitPolice)
                                                .Where(s => s.Id == breach.AssignmentTicketId).FirstOrDefaultAsync(); 

            
            //var orderLast = await _context.OrderSales.Where(o => o.IdOrigin == breach.Id).FirstOrDefaultAsync();

            //if (orderLast.Status == "EOS01")
            //{
            //    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "No se puede agregar la infracción por el estado de estatus" });

            //}

            //var valueUnity = _context.

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var ststus = new List<string>() { "EO003", "EOS02" };
                    var orderOld = _context.OrderSales.Where(x => x.Folio == breach.Folio && !ststus.Contains(x.Status)).FirstOrDefault() ;
                    if (orderOld != null) {
                        orderOld.ExpirationDate = DateTime.Now.AddDays(-1);
                        orderOld.Status = "EOS04";
                    }

                    OrderSale order = new OrderSale();

                    //Temporal en lo que se crea trigger de asignacion de folio
                    order.Folio =  breach.Folio;
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
                    order.Observation = "FOLIO " +":"+" "+ breach.Series+ticket.Folio+","+"AGENTE "+ ":"+" "+ ticket.TransitPolice.Plate;
                    order.ExpirationDate = DateTime.UtcNow.ToLocalTime();
                    var valExpirationDate = Convert.ToInt32(param.NumberColumn);
                    var endDate = order.ExpirationDate.AddDays(valExpirationDate);


                    var valueDate = endDate.DayOfYear;
                    var valDate = order.DateOrder.DayOfYear;
                    order.ExpirationDate = endDate;

                    DateTime fInicio = DateTime.UtcNow.ToLocalTime();
                    DateTime fFinal = endDate.Date;
                    if (fFinal < fInicio)
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


                        var article =  _context.BreachArticles.Where( v => v.Id == x.BreachList.BreachArticleId).FirstOrDefault();
                        var list = _context.BreachLists.Where(xa => xa.BreachArticleId == article.Id).FirstOrDefault();
                        var  na = "Articulo";


                        OrderSaleDetail orderSaleDetail = new OrderSaleDetail
                        {
                            
                            Unity = "SAN",
                            UnitPrice = factor.NumberColumn,
                            HaveTax = false,
                            Description = na +' ' +article.Article + ' ' + x.BreachList.Fraction +' '+ x.BreachList.Description,
                            CodeConcept = serParam.TextColumn,
                            NameConcept = x.BreachList.Description,
                            Amount = x.Amount,
                            OnAccount = 0,
                            Quantity = x.Amount/factor.NumberColumn,
                            idBreachList = list.Id

                        };
                        orderSaleDetails.Add(orderSaleDetail);
                    });

                    if (!string.IsNullOrEmpty( breach.CodeConceptArrastre) ) {
                        var Arrastre = _context.Products.Where(x => x.Id.ToString() == breach.CodeConceptArrastre).FirstOrDefault();
                        var Parent = _context.Products.Where(x => x.Id == Arrastre.Parent).FirstOrDefault();
                        var SuperParent = _context.Products.Where(x => x.Id == Parent.Parent).FirstOrDefault();
                        var TariffArrastre = _context.TariffProducts.Where(x => x.ProductId.ToString() == breach.CodeConceptArrastre).FirstOrDefault();
                        orderSaleDetails.Add(
                            new OrderSaleDetail
                            {

                                Unity = "SAN",
                                UnitPrice = TariffArrastre.Amount,
                                HaveTax = TariffArrastre.HaveTax,
                                Description = "Arratres " + SuperParent.Name + " - " + Parent.Name + " - " + Arrastre.Name,
                                CodeConcept = Arrastre.Id.ToString(),
                                NameConcept = "Arratres " + SuperParent.Name + " - " + Parent.Name + " - " + Arrastre.Name,
                                Amount = TariffArrastre.Amount,
                                OnAccount = 0,
                                Quantity = 1,
                                idBreachList = 0

                            }
                            );
                        order.Amount = order.Amount + TariffArrastre.Amount;
                    }

                    if (!string.IsNullOrEmpty(breach.CodeConceptDaysCorralon))
                    {
                        var days = DateTime.Now - breach.DateBreach ;
                        var dias = days.Days +1 ;
                        var Corralon = _context.Products.Where(x => x.Id.ToString() == breach.CodeConceptDaysCorralon).FirstOrDefault();
                        var Parent = _context.Products.Where(x => x.Id == Corralon.Parent).FirstOrDefault();
                        var SuperParent = _context.Products.Where(x => x.Id == Parent.Parent).FirstOrDefault();
                        var TariffCorralon = _context.TariffProducts.Where(x => x.ProductId.ToString() == breach.CodeConceptArrastre).FirstOrDefault();
                        breach.DaysCorralon = dias.ToString();
                        orderSaleDetails.Add(
                            new OrderSaleDetail
                            {

                                Unity = "SAN",
                                UnitPrice = TariffCorralon.Amount,
                                HaveTax = TariffCorralon.HaveTax,
                                Description = "Corralon " + SuperParent.Name + " - " + Parent.Name + " - " + Corralon.Name +" - "+ dias + " Días",
                                CodeConcept = Corralon.Id.ToString(),
                                NameConcept = "Corralon " + SuperParent.Name + " - " + Parent.Name + " - " + Corralon.Name +" - "+ dias + " Días",
                                Amount = TariffCorralon.Amount * dias,
                                OnAccount = 0,
                                Quantity = dias,
                                idBreachList = 0

                            }
                            );
                        order.Amount = order.Amount + (TariffCorralon.Amount * dias);
                    }

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

            return StatusCode((int)TypeError.Code.Ok, new { Success = "Orden de cobro generada" });
        }


        [HttpGet("GetStatus")]
        public IEnumerable<Status> GetStatus()
        {
            return _context.Statuses.Where(d => d.GroupStatusId == 7);
        }

        [HttpGet("GetList")]
        public IEnumerable<BreachList> GetList()
        {
            return _context.BreachLists.Include(x => x.BreachArticle).OrderBy(a => a.Description);

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


        [HttpGet("SearchTicket/{license}")]
        public async Task<IActionResult> GetTicketLicense([FromRoute] int license)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var breach = await _context.Breaches.Where(l => l.AssignmentTicketId == license && l.Series == "C").ToListAsync();


            if (breach.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest,
                                   new { Error = "No hay infracción" });
            }

            return Ok(breach);
        }

        //POST: API/BREACH
        [HttpPost("{idsInfra?}/{Cobro?}")]
        public async Task<IActionResult> PostBreach(int BreachId, [FromBody] Breach breanch, [FromRoute] string idsInfra = "", [FromRoute] string Cobro= "")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Cobro = "2";

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
                        NewBreach.Status = "INF02";
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
                        NewBreach.code_marca = breanch.code_marca;
                        NewBreach.CodeConceptArrastre = breanch.CodeConceptArrastre;
                        NewBreach.DaysCorralon = breanch.DaysCorralon;
                        NewBreach.CodeConceptDaysCorralon = breanch.CodeConceptDaysCorralon;
                        _context.Breaches.Add(NewBreach);
                        //await _context.SaveChangesAsync();


                        foreach(var wa in breanch.BreachWarranties)
                        {
                            BreachWarranty warranty = new BreachWarranty
                            {
                                BreachId = NewBreach.Id,
                                WarrantyId = wa.WarrantyId,
                                References = wa.References,
                                Observations = wa.Observations,
                            };

                            _context.Add(warranty);
                            _context.SaveChanges();

                        }

                        //BreachWarranty warranty = new BreachWarranty
                        //{
                        //    BreachId = NewBreach.Id,
                        //    WarrantyId = breanch.BreachWarranties,
                        //    References = breanch.BreachWarranties,
                        //    Observations = NewBreach.Place
                        //};

                        //_context.Add(warranty);
                        //_context.SaveChanges();



                        decimal sumBreanch = 0;
                        var idsBreachList = idsInfra.Split(",").ToList();

                        foreach (var list in breanch.BreachDetails)
                        {
                            var value = await _context.BreachLists.Where(b => b.Id == list.BreachListId).FirstOrDefaultAsync();

                            //value.HaveBonification = false;


                            var getLicense = await _context.Breaches.Where(z => z.LicensePlate.Contains(breanch.LicensePlate)).ToListAsync();
                            int cont = getLicense.Count;


                            if (Cobro == "1" && idsBreachList.Contains(list.BreachListId.ToString()))
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
                                    TimesFactor = valueJudge,

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
                        NewBreach.Status = "INF02";
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
                        NewBreach.code_marca = breanch.code_marca;
                        NewBreach.CodeConceptArrastre = breanch.CodeConceptArrastre;
                        NewBreach.DaysCorralon = breanch.DaysCorralon;
                        NewBreach.CodeConceptDaysCorralon = breanch.CodeConceptDaysCorralon;

                        _context.Breaches.Add(NewBreach);

                        foreach (var wa in breanch.BreachWarranties)
                        {
                            BreachWarranty warranty = new BreachWarranty
                            {
                                BreachId = NewBreach.Id,
                                WarrantyId = wa.WarrantyId,
                                References = wa.References,
                                Observations = wa.Observations,
                            };

                            _context.Add(warranty);
                            _context.SaveChanges();

                        }


                        decimal sumBreanch = 0;

                        foreach (var list in breanch.BreachDetails)
                        {
                            var value = await _context.BreachLists.Where(b => b.Id == list.BreachListId).FirstOrDefaultAsync();
                            value.HaveBonification = false;

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


        [HttpGet("CheckIsMaxInfraccion/{infraccion_id}/{placa}")]
        public async Task<IActionResult> CheckIsMaxInfraccion([FromRoute] string infraccion_id, [FromRoute] string placa)
        {
            var id_infracciones =  infraccion_id.Split("acute;").ToList();
            var result = _context.Breaches.Include(x =>x.BreachDetails).Where(bl => bl.LicensePlate == placa).ToList();
            List<string> messages = new List<string>();
            List<string> idsApply = new List<string>();
            List<BreachDetail> breanchList = new List<BreachDetail>();
            if (result.Count > 0) {
                result.ForEach(x => {
                    breanchList.AddRange(x.BreachDetails);

                });

                var BreachListR = breanchList.Where(x => id_infracciones.Contains(x.BreachListId.ToString())).Select(x => x.BreachListId).Distinct().ToList();
               // BreachListR = BreachListR.Select(x =>  x.BreachListId).Distinct().ToList();

                if (BreachListR.Count >0)
                {
                    BreachListR.ForEach(x =>
                    {
                        var breacListt = _context.BreachLists.Where(bl => bl.Id == x).FirstOrDefault() ;
                        idsApply.Add(breacListt.Id.ToString());
                        messages.Add("Fracción "+breacListt.Fraction+ " "+breacListt.Description );
                    });
                      
                }
            }
            return Ok(new {messages , idsApply });

        }

        [HttpGet("GetProducsArrastre")]
        public async Task<IActionResult> GetProducsArrastre()
        {
            var products = _context.Products.Where(x => x.Parent == 3182).ToList();
            return Ok(products);
        }


        [HttpGet("GetProducsCorralon")]
        public async Task<IActionResult> GetProducsCorralon()
        {
            var products = _context.Products.Where(x => x.Parent == 3189).ToList();
            return Ok(products);
        }


        [HttpGet("findBreachPlaca/{placa}")]
        public async Task<IActionResult> FindBreachPlaca(string placa)
        {
            var breach = _context.Breaches
                .Include(x => x.BreachDetails)
                .Include(x => x.BreachWarranties)
                .Include(x => x.TaxUser).ThenInclude(x => x.TaxAddresses).Where(x => x.LicensePlate == placa).LastOrDefault();
            return Ok(breach);
        }
    }
}
