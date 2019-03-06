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


        public BreachController(ApplicationDbContext context, UserManager<ApplicationUser>userManager)
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

            var breach = await _context.Breaches.FindAsync(id);

            if (breach == null)
            {
                return NotFound();
            }

            return Ok(breach);
        }


        [HttpGet("GetStatus")]
        public  IEnumerable<Status> GetStatus(){
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

            if (breach == null)
            {
                return NotFound();
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

            if((breanch.BreachDetails != null) && ((breanch != null) && (breanch.TaxUserId == 0)))
            {
                TaxUser tax = new TaxUser
                {
                    Name = breanch.TaxUser.Name,
                    RFC = breanch.TaxUser.RFC,
                    CURP = breanch.TaxUser.CURP,
                    PhoneNumber = breanch.TaxUser.PhoneNumber,
                    EMail = breanch.TaxUser.EMail,
                    IsActive = breanch.TaxUser.IsActive
                };

                TaxAddress address = new TaxAddress();

                address.Street = breanch.TaxUser.TaxAddresses.FirstOrDefault().Street;
                address.Outdoor = breanch.TaxUser.TaxAddresses.FirstOrDefault().Outdoor;
                address.Indoor = breanch.TaxUser.TaxAddresses.FirstOrDefault().Indoor;
                address.Zip = breanch.TaxUser.TaxAddresses.FirstOrDefault().Zip;
                address.Suburb = breanch.TaxUser.TaxAddresses.FirstOrDefault().Suburb;
                address.Town = breanch.TaxUser.TaxAddresses.FirstOrDefault().Town;
                address.State = breanch.TaxUser.TaxAddresses.FirstOrDefault().State;


                tax.TaxAddresses.Add(address);
                _context.TaxUsers.Add(tax);

                foreach (var list in breanch.BreachDetails){
                   var value = await _context.BreachLists.Where(b => b.Id == list.BreachListId).ToListAsync();

                    BreachDetail newBreachDetail = new BreachDetail();
                    if (breanch.LicensePlate == null){
                        //newBreachDetail.AplicationDays = 
                    }


                }


                var param = await _context.SystemParameters
                                    .Where(x => x.Name == "Factor").FirstOrDefaultAsync();
                if (param != null)
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de salario minimo") });
                breanch.TaxUser = tax;

                var a = _context.BreachLists.Find(breanch.BreachDetails.FirstOrDefault().BreachListId);

                //BreachDetail breachlist = new BreachDetail();

                //List<BreachDetail> breachlist = new List<BreachDetail>();
                //breachlist = breanch.BreachDetails;


                //breachDetail.AplicationDays = breanch.BreachDetails.FirstOrDefault().AplicationDays;
                //breachDetail.Amount = breanch.BreachDetails.FirstOrDefault().Amount;
                //breachDetail.PercentBonification = breanch.BreachDetails.FirstOrDefault().PercentBonification;
                //breachDetail.Bonification = breanch.BreachDetails.FirstOrDefault().Bonification;
                //breachDetail.BreachListId = breanch.BreachDetails.FirstOrDefault().BreachListId;
                //breachDetail.BreachList = a;
                ////breachDetail.BreachId = NewBreach.Id;

                //breanch.BreachDetails.Add(breachDetail);
                //_context.BreachDetails.Add(breachDetail);
                //_context.SaveChanges();
                              

                NewBreach.TaxUserId = tax.Id;
                NewBreach.Car = breanch.Car;
                NewBreach.Series = breanch.Series;
                NewBreach.Folio = breanch.Folio;
                NewBreach.CaptureDate = breanch.CaptureDate;
                NewBreach.Place = breanch.Place;
                NewBreach.Sector = breanch.Sector;
                NewBreach.Zone = breanch.Zone;
                NewBreach.TypeCar = breanch.TypeCar;
                NewBreach.Service = breanch.Service;
                NewBreach.Color = breanch.Color;
                NewBreach.Reason = breanch.Reason;
                NewBreach.LicensePlate = breanch.LicensePlate;
                NewBreach.Reason = breanch.Reason;
                NewBreach.Judge = breanch.Judge;
                NewBreach.DateBreach = breanch.DateBreach;
                NewBreach.Status = breanch.Status;
                NewBreach.AssignmentTicketId = breanch.AssignmentTicketId;
                NewBreach.UserId = breanch.UserId;

                _context.Breaches.Add(NewBreach);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBreach", new { id = NewBreach.Id }, NewBreach);





            }
            else
            {
                var taxu = await _context.TaxUsers.FindAsync(breanch.TaxUserId);

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        NewBreach.TaxUserId = taxu.Id;
                        NewBreach.Car = breanch.Car;
                        NewBreach.Series = breanch.Series;
                        NewBreach.Folio = breanch.Folio;
                        NewBreach.CaptureDate = breanch.CaptureDate;
                        NewBreach.Place = breanch.Place;
                        NewBreach.Sector = breanch.Sector;
                        NewBreach.Zone = breanch.Zone;
                        NewBreach.TypeCar = breanch.TypeCar;
                        NewBreach.Service = breanch.Service;
                        NewBreach.Color = breanch.Color;
                        NewBreach.Reason = breanch.Reason;
                        NewBreach.LicensePlate = breanch.LicensePlate;
                        NewBreach.Reason = breanch.Reason;
                        NewBreach.Judge = breanch.Judge;
                        NewBreach.DateBreach = breanch.DateBreach;
                        NewBreach.Status = breanch.Status;
                        NewBreach.AssignmentTicketId = breanch.AssignmentTicketId;
                        NewBreach.UserId = breanch.UserId;


                        _context.Breaches.Add(NewBreach);
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

       

        // PUT: api/Breach/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBreach([FromRoute] int id, [FromBody] Breach breach)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            if (id != breach.Id)
            {
                return BadRequest();
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
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
            }catch(Exception e){
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(breach);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para editar la infracción" });


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
