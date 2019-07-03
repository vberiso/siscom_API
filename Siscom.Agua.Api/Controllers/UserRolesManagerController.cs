using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
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


        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {

            List<ViewUsersVM> newlistUsers = new List<ViewUsersVM>();
            var users = _context.Users.ToList();

            foreach (var list in users)
            {
                if (list.DivitionId == 0)
                {
                    var divition = "SIN DIVISION";
                    //var idnameRol = _context.UserRoles.Where(n => n.UserId == list.Id).FirstOrDefault();
                    var temp = _context.UserRoles.FirstOrDefault(x => x.UserId.Equals(list.Id));
                    if(temp != null)
                    {
                        var nameRol = _context.Roles.FirstOrDefault(r => r.Id.Equals(temp.RoleId));


                        //newlistUsers.lastname = list.LastName;
                        //newlistUsers.secondlastname = list.SecondLastName;
                        //newlistUsers.name = list.Name;
                        //newlistUsers.divitionName = divition;
                        //newlistUsers.id = list.Id;
                        //newlistUsers.rolName = nameRol.Name;

                        newlistUsers.Add(new ViewUsersVM()
                        {
                            lastname = list.LastName,
                            secondlastname = list.SecondLastName,
                            name = list.Name,
                            divitionName = divition,
                            id = list.Id,
                            nameRol = nameRol.Name,
                        });
                    }                    
                }
                else
                {
                    var divition = _context.Divisions.Where(a => a.Id == list.DivitionId).FirstOrDefault();
                    //var idnameRol = _context.UserRoles.Where(n => n.UserId == list.Id).FirstOrDefault();
                    var temp = _context.UserRoles.FirstOrDefault(x => x.UserId.Equals(list.Id));
                    if(temp != null)
                    {
                        var nameRol = _context.Roles.FirstOrDefault(r => r.Id.Equals(temp.RoleId));

                        newlistUsers.Add(new ViewUsersVM()
                        {
                            lastname = list.LastName,
                            secondlastname = list.SecondLastName,
                            name = list.Name,
                            divitionName = divition.Name,
                            id = list.Id,
                            nameRol = nameRol.Name,
                        });
                    }
                    //newlistUsers.lastname = list.LastName;
                    //newlistUsers.secondlastname = list.SecondLastName;
                    //newlistUsers.name = list.Name;
                    //newlistUsers.divitionName = divition.Name;
                    //newlistUsers.id = list.Id;
                    //newlistUsers.rolName = "prueba";
                }
            }
            return Ok(newlistUsers);
        }

        [HttpPut("changeRol/{id}")]
         
        public async Task<IActionResult> PutBreachList([FromRoute] int id, [FromBody] ApplicationUser rol )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    if (id != rol.DivitionId)
                    {
                        return BadRequest();
                    }

                    _context.Entry(rol).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DivisionExist(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    scope.Complete();
                    return Ok(rol);

                }


            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(rol);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para cambiar la division" });
            }



            return Ok(rol);
        }

        private bool DivisionExist(int id)
        {
            return _context.Users.Any(e => e.DivitionId == id);
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
