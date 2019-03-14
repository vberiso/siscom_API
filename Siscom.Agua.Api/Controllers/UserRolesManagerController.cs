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
    [Authorize(Policy = "RequireAdminRole")]
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


        [HttpPost]
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

            result = await UserManager.CreateAsync(user, CrearPassword(5));
            await UserManager.AddToRoleAsync(user, addUser.RoleName);

            if (result.Succeeded)
            {
                return StatusCode((int)TypeError.Code.Ok, new { Error = "Usuario creado con éxito" });
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
            //if (!result.Succeeded)
            //{
            //    //throw exception......
            //}
            return Ok();
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
        [DataType(DataType.Password), Required(ErrorMessage = "Old Password Required")]
        public string oldPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "New Password Required")]
        public string newPassword { get; set; }
    }
}
