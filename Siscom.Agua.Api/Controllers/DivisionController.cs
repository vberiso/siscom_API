using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Siscom.Agua.Api.Controllers
{   
    [Route("api/Division")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class DivisionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DivisionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Report
        [HttpGet()]
        public async Task<IActionResult> GetDivision()
        {
            var Divisions = await _context.Divisions.ToListAsync();
                        
            if (Divisions == null)
            {
                return NotFound();
            }
            return Ok(Divisions);
        }

        [HttpGet("FromApp/{id}")]
        public async Task<IActionResult> GetFromApp([FromRoute] int id)
        {
            var Divisions = await _context.Divisions.Where(d => d.IdSolution == id && d.IsActive == true).ToListAsync();

            if (Divisions == null)
            {
                return NotFound();
            }
            return Ok(Divisions);
        }


        [HttpGet("roles")]
        public IEnumerable<ApplicationRol> GetRoles()
        {
            return _context.Roles;
        }


    }
}