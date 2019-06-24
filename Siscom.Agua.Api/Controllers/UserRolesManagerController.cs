using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserRolesManagerController : ControllerBase
    {
        private RoleManager<ApplicationRol> RoleManager;
        private UserManager<ApplicationUser> UserManager;
        private readonly ApplicationDbContext _context;

        public UserRolesManagerController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            RoleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRol>>();
            UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this._context = context;
        }


        [HttpPost("AddUsers")]
        public async Task<IActionResult> Post([FromBody] AddUserVM addUser)
        {
            IdentityResult result;
            ApplicationUser user = new ApplicationUser()
            {
                Email = addUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = addUser.UserName,
                Name = addUser.Name,
                LastName = addUser.LastName,
                SecondLastName = addUser.SecondLastName,
                DivitionId = addUser.DivitionId,
                IsActive = true
            };
            string password = CrearPassword(6);
            result = await UserManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });
            }
            result = await UserManager.AddToRoleAsync(user, addUser.RoleName);

            if (!result.Succeeded)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });
            }

            if (result.Succeeded)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Usuario creado con éxito Contraseña: " + password  });
            }

            return BadRequest();
        }

        [HttpPost("AddUsersTransitPolice")]
        public async Task<IActionResult> AddTransitPolice ([FromBody] TransitPoliceVM addUser)
        {
            IdentityResult result;
            ApplicationUser user = new ApplicationUser()
            {
                Email = addUser.EMail,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = addUser.UserName,
                Name = addUser.Name,
                LastName = addUser.LastName,
                SecondLastName = addUser.SecondLastName,
                DivitionId = 14,
                IsActive = true
            };
            string password = CrearPassword(5);
            result = await UserManager.CreateAsync(user, password);
            await UserManager.AddToRoleAsync(user, "Agente");

            if (result.Succeeded)
            {
                TransitPolice police = new TransitPolice
                {
                    Name = addUser.Name,
                    LastName = addUser.LastName,
                    SecondLastName = addUser.SecondLastName,
                    EMail = addUser.EMail,
                    PhoneNumber = addUser.PhoneNumber,
                    Plate = addUser.Plate,
                    IsActive = true,
                    Address = addUser.Address,
                    User = user,
                    UserId = user.Id,
                };

                await _context.TransitPolices.AddAsync(police);
                await _context.SaveChangesAsync();
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Usuario creado con éxito Contraseña [" + password + "]" });
            }
            else
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(" ", result.Errors) });
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePwdViewModel usermodel)
        {
            var userId = User.Claims.ToList()[3].Value;
            var user = await UserManager.FindByIdAsync(userId);
            var result = await UserManager.ChangePasswordAsync(user, usermodel.oldPassword, usermodel.newPassword);
            if (!result.Succeeded)
            {
                //throw exception......
                return BadRequest( new { Error = "Error al intentar cambiar la contraseña favor de intentarlo mas tarde. Detalles de error: " + string.Join(" ", result.Errors.ToList()) });
            }
            return Ok();
        }

        [HttpGet("GetUserByRoleName/{RoleName}")]
        public async Task<IActionResult> GetUserByRoleName([FromRoute] string RoleName)
        {
            return Ok(UserManager.GetUsersInRoleAsync(RoleName).Result);
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetUser()
        {
            var idRole = _context.Roles.Where(r => r.Name == "User").FirstOrDefault();
            var lstUsersInRol = _context.UserRoles.Where(ur => ur.RoleId == idRole.Id).Select(ur => ur.UserId).ToList();
            var res = _context.Users.Where(u => lstUsersInRol.Contains(u.Id)).Select(x => new { Id = x.Id, Name = x.Name, LastName = x.LastName, SecondLastName = x.SecondLastName }  ).ToList();
            return Ok(res);
        }

        private string CrearPassword(int longitud)
        {
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
    }

    public class ChangePwdViewModel
    {
        [DataType(DataType.Password), Required(ErrorMessage = "Contraseña actual Requerida")]
        public string oldPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Nueva Contraseña Requerida")]
        public string newPassword { get; set; }
    }
}
