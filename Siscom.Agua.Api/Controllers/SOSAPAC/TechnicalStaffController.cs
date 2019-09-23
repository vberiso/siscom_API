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

            var Staff = _context.TechnicalStaffs.Where(x => x.Id.ToString() == staffId).Include(x => x.OrderWorks).First();
            List<Agreement> Agreements = new List<Agreement>();
            foreach (var orderWork in Staff.OrderWorks)
            {
                orderWork.Agreement = 
                    _context.Agreements.Where(x => x.Id == orderWork.AgrementId)
                    .Include(x => x.OrderWork)
                    
                    .Include(x => x.Clients)
                    .Include(x => x.Addresses)
                        .ThenInclude(x => x.Suburbs)
                            .ThenInclude(x => x.Towns)
                                .ThenInclude(x => x.States)
                                    .ThenInclude(x => x.Countries)
                                        .First()
                    ;
            }



            return Ok(Staff.OrderWorks);
        }

        [HttpGet("Staffs/{id?}")]
        public async Task<IActionResult> GetStaffs([FromRoute] string id = null)
        {
            if (id == null)
            {
                var Staffs = _context.TechnicalStaffs
                    .Include(x => x.TechnicalRole)
                
                                       
                    .Include(x => x.TechnicalTeam).ToList();
               
                return Ok(Staffs);
            }
            var Staff = _context.TechnicalStaffs.Where(x => x.Id.ToString() == id)
                .Include(x => x.TechnicalRole)
                    .Include(x => x.TechnicalTeam)
                .First();
            return Ok(Staff);
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
