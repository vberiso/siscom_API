using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers.SOSAPAC
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    
    [Authorize]
    public class TechnicalStaffController : ControllerBase
    {



        private readonly ApplicationDbContext _context;

        public TechnicalStaffController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet("getAgremmentsOfStaff/{staffId}")]
        public async Task<IActionResult> getAgremmentsOfStaff([FromRoute] string staffId)
        {
            DateTime DiaActual = DateTime.Now;
            int delta = DayOfWeek.Monday - DiaActual.DayOfWeek;
            DateTime LunesSemanaPasada = DiaActual.AddDays(delta - 7);

            //var Staff = _context.TechnicalStaffs.Where(x => x.Id.ToString() == staffId).Include(x => x.OrderWorks).First();
            var Staff = _context.TechnicalStaffs.Where(x => x.Id.ToString() == staffId).First();
            Staff.OrderWorks = await _context.OrderWorks.Where(ow => ow.TechnicalStaffId == Staff.Id && ow.DateOrder >= LunesSemanaPasada).ToListAsync();

            List<Agreement> Agreements = new List<Agreement>();
            foreach (var orderWork in Staff.OrderWorks)
            {
                if(orderWork.AgrementId != 0)
                {
                    orderWork.Agreement =
                    _context.Agreements.Where(x => x.Id == orderWork.AgrementId)
                    //.Include(x => x.OrderWork)

                    .Include(x => x.Clients)
                    .Include(x => x.Addresses)
                        .ThenInclude(x => x.Suburbs)
                            .ThenInclude(x => x.Towns)
                                .ThenInclude(x => x.States)
                                    .ThenInclude(x => x.Countries)
                                        .First()
                    ;
                }
                else if(orderWork.TaxUserId != 0)
                {
                    orderWork.Agreement = _context.Agreements.Where(x => x.Id == orderWork.AgrementId)
                                                            .Include(x => x.Clients)
                                                            .Include(x => x.Addresses)                                                                
                                                            .First();
                    orderWork.Agreement.Clients.Clear();
                    orderWork.Agreement.Clients.Add( _context.TaxUsers.Where(x => x.Id == orderWork.TaxUserId)
                        .Select(c => new Client() { Id = 0, TypeUser = "CLI01", Name = c.Name, LastName = "" } )
                        .First());
                    orderWork.Agreement.Addresses.Clear();
                    orderWork.Agreement.Addresses.Add(_context.TaxAddresses
                        .Where(x => x.TaxUserId == orderWork.TaxUserId)
                        .Select(d => new Address() { Id = 0, TypeAddress = "DIR01", Street = d.Street, Outdoor = d.Outdoor, Indoor = d.Indoor, Suburbs = new Suburb() { Id = 0, Name = d.Suburb, Towns = new Town() { Id = 0, Name = d.Town } } })
                        .First());
                }
                else
                {
                    orderWork.Agreement = _context.Agreements.Where(x => x.Id == orderWork.AgrementId)
                                                            .Include(x => x.Clients)
                                                            .Include(x => x.Addresses)
                                                            .First();
                    orderWork.Agreement.Clients.Clear();
                    orderWork.Agreement.Clients.Add( new Client() { Id = 0, TypeUser = "CLI01", Name = "Multipes cuentas", LastName = "" });
                    orderWork.Agreement.Addresses.Clear();
                    orderWork.Agreement.Addresses.Add( new Address() { 
                        Id = 0, 
                        TypeAddress = "DIR01", 
                        Street = "Multiple", 
                        Outdoor = "...", 
                        Indoor = "...", 
                        Suburbs = new Suburb() { Id = 0, Name = "Multiple", 
                            Towns = new Town() { Id = 0, Name = "Multiple" } 
                        } 
                    });
                }
                
            }



            return Ok(Staff.OrderWorks);
        }

        [HttpGet("Staffs/{id?}")]
        public async Task<IActionResult> GetStaffs([FromRoute] string id = null)
        {
            List<Siscom.Agua.Api.Model.TechnicalStaffVM> lst = new List<Model.TechnicalStaffVM>();
            if (id == null)
            {
                var Staffs = _context.TechnicalStaffs
                    .Include(x => x.TechnicalRole)
                    .Include(x => x.TechnicalTeam).ToList();

                foreach (var tmpStaff in Staffs)
                {
                    var tmpUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == tmpStaff.UserId);
                    Siscom.Agua.Api.Model.TechnicalStaffVM tmpTSvm = new Model.TechnicalStaffVM()
                    {
                        Id = tmpStaff.Id,
                        Name = tmpStaff.Name,
                        Phone = tmpStaff.Phone,
                        IsActive = tmpStaff.IsActive,
                        TechnicalRoleId = tmpStaff.TechnicalRoleId,
                        TechnicalRole = tmpStaff.TechnicalRole,
                        TechnicalTeamId = tmpStaff.TechnicalTeamId,
                        TechnicalTeam = tmpStaff.TechnicalTeam,
                        Nick = tmpUser != null ? tmpUser.UserName : "",
                        IMEI = "",
                        Email = tmpUser != null ? tmpUser.Email : "",
                        DivisionId = tmpUser != null ? tmpUser.DivitionId : 0,
                        UserId = tmpUser != null ? tmpUser.Id : "",
                        Division = tmpUser != null ? _context.Divisions.FirstOrDefault(x => x.Id == tmpUser.DivitionId) : null
                    };
                    lst.Add(tmpTSvm);
                }

                return Ok(lst);
            }
            var Staff = _context.TechnicalStaffs.Where(x => x.Id.ToString() == id)
                .Include(x => x.TechnicalRole)
                    .Include(x => x.TechnicalTeam)
                .First();

            var userOT = await _context.Users.FirstOrDefaultAsync(x => x.Id == Staff.UserId);
            
            Siscom.Agua.Api.Model.TechnicalStaffVM technicalStaffVM = new Model.TechnicalStaffVM()
            {
                Id = Staff.Id,
                Name = Staff.Name,
                Phone = Staff.Phone,
                IsActive = Staff.IsActive,
                TechnicalRoleId = Staff.TechnicalRoleId,
                TechnicalTeamId = Staff.TechnicalTeamId,
                Nick = userOT != null ? userOT.UserName : "",
                IMEI = "",
                Email = userOT != null ? userOT.Email : "",
                DivisionId = userOT != null ? userOT.DivitionId : 0,
                UserId = userOT != null ? userOT.Id : "",
                Division = userOT != null ? _context.Divisions.FirstOrDefault(x => x.Id == userOT.DivitionId) : null
            };

            return Ok(technicalStaffVM);
        }

        [HttpPost("Staffs")]
        public async Task<IActionResult> SetStaff([FromBody] TechnicalStaff data)
        {
            try
            {
                _context.TechnicalStaffs.Add(data);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se guardaron correctamente", ID = data.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("UpdateStaffs/{id}")]
        public async Task<IActionResult> UpdateRoles([FromRoute] int id, [FromBody] TechnicalStaff data)
        {
            try
            {
                var staff = _context.TechnicalStaffs.Where(x => x.Id == id).First();
                staff.Name = data.Name;
                staff.IsActive = data.IsActive;
                staff.Phone = data.Phone;
                staff.TechnicalRoleId = data.TechnicalRoleId;
                staff.TechnicalTeamId = data.TechnicalTeamId;

                _context.TechnicalStaffs.Update(staff);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }

        [HttpPost("DeleteStaffs/{id}")]
        public async Task<IActionResult> DeleteRoles([FromRoute] int id)
        {
            try
            {
                var staff = _context.TechnicalStaffs.Where(x => x.Id == id).First();

                _context.TechnicalStaffs.Remove(staff);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new { msg = "El elemento se elimino correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

        }
    }
}
