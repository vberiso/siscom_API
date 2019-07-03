using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize(Policy = "RequireAdminRole")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        RoleManager<ApplicationRol> RoleManager;
        IdentityResult roleResult;

        public RolesController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            RoleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRol>>();
        }

        // GET: api/ApplicationRols
        [HttpGet]
        public IEnumerable<ApplicationRol> GetRoles()
        {
            return _context.Roles;
        }

        // GET: api/ApplicationRols/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationRol([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationRol = await _context.Roles.FindAsync(id);

            if (applicationRol == null)
            {
                return NotFound();
            }

            return Ok(applicationRol);
        }

        // PUT: api/ApplicationRols/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationRol([FromRoute] string id, [FromBody] ApplicationRoleVM applicationRol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != applicationRol.Id)
            {
                return BadRequest();
            }
            var role = RoleManager.Roles.First(r => r.Id == id);

            role.Name = applicationRol.RoleName;
            role.IsActive = applicationRol.IsActive;

            roleResult =  await RoleManager.UpdateAsync(role);
            if (roleResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                return StatusCode((int)TypeError.Code.InternalServerError,
                                  new { Error = "Error al actualizar el role. Detalles del error: " + string.Join(" ", roleResult.Errors) });
            }

          
        }

        // POST: api/ApplicationRols
        [HttpPost]
        public async Task<IActionResult> PostApplicationRol([FromBody] ApplicationRoleVM applicationRol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var roleExist = await RoleManager.RoleExistsAsync(applicationRol.RoleName);

            if (!roleExist)
            {
                roleResult = await RoleManager.CreateAsync(new ApplicationRol(applicationRol.RoleName));
                if (roleResult.Succeeded)
                {
                    return CreatedAtAction("GetApplicationRol", new { @id = applicationRol.Id, RoleName = applicationRol.RoleName });
                }
                else
                {
                    return StatusCode((int)TypeError.Code.InternalServerError,
                                  new { Error = "Error al crear el role. Detalles del error: " + string.Join(" ", roleResult.Errors ) });
                }
            }
            else
            {
                return StatusCode((int)TypeError.Code.Conflict,
                                  new { Error = "El rol ya existe favor de verificar" });
            }
        }

        //// DELETE: api/ApplicationRols/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteApplicationRol([FromRoute] string id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var applicationRol = await _context.Roles.FindAsync(id);
        //    if (applicationRol == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Roles.Remove(applicationRol);
        //    await _context.SaveChangesAsync();

        //    return Ok(applicationRol);
        //}

        private bool ApplicationRolExists(string id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}