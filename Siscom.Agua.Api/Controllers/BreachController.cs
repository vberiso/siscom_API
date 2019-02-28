using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
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


        // GET: api/Warranty
        [HttpGet]
        public IEnumerable<Breach> GetBreach()
        {
            return _context.Breaches;
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


            var taxu = await _context.TaxUsers.FindAsync(breanch.TaxUserId);
            //var user = await _context.Breaches.FindAsync(breanch.UserId);



            //Breach NewBreach = new Breach(){
            //    Series             = breanch.Series,
            //    Folio              = breanch.Folio,
            //    CaptureDate        = breanch.CaptureDate,
            //    Place              = breanch.Place,
            //    Sector             = breanch.Sector,
            //    Zone               = breanch.Zone,
            //    Car                = breanch.Car,
            //    TypeCar            = breanch.TypeCar,
            //    Service            = breanch.Service,
            //    Color              = breanch.Color,
            //    LicensePlate       = breanch.LicensePlate,
            //    Reason             = breanch.Reason,
            //    Judge              = breanch.Judge,
            //    DateBreach         = breanch.DateBreach,
            //    Status             = breanch.Status,
            //    AssignmentTicketId = breanch.AssignmentTicketId,
            //    UserId             = breanch.UserId,
            //    User               = await userManager.FindByIdAsync(breanch.UserId)

            //};

            NewBreach.TaxUser = taxu;
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

            return Ok(breach);
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
