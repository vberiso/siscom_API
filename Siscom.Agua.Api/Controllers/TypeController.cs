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

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class TypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Group/{id}")]
        public async Task<IActionResult> GetTypeGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = await _context.Types
                                     .Where(x => x.GroupTypeId == id).ToListAsync();

            if (type == null)
            {
                return NotFound();
            }

            return Ok(type);
        }

        [HttpGet("ByToolCode/{code}")]
        public async Task<IActionResult> GetByToolCode([FromRoute] string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var groupTypes = await _context.GroupTypes
                .Include(x => x.Types)
                .Where(gt => gt.Observations.Contains(code)).ToListAsync();

            //var type = await _context.Types
            //                        .Include(x => x.GroupType)
            //                        .Where(x => x.GroupType.Observations.Contains(code)).ToListAsync();

            if(groupTypes == null)
            {
                return NotFound();
            }

            return Ok(groupTypes);
        }
    }
}