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

        [HttpPost("AddUsersOT")]
        public async Task<IActionResult> PostAddUserOT([FromBody] TechnicalStaffVM addUser)
        {
            IdentityResult result;            
            string password = CrearPassword(6);
            ApplicationUser user = new ApplicationUser()
            {                
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = addUser.Nick,
                Name = addUser.Name,
                Email = addUser.Email,
                PhoneNumber = addUser.Phone,
                LastName = password,
                SecondLastName = "OT",
                DivitionId = addUser.DivisionId,
                IsActive = addUser.IsActive
            };            
            result = await UserManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });
            }

            var tmpRol = await  _context.TechnicalRoles.FirstOrDefaultAsync(t => t.Id == addUser.TechnicalRoleId);
            string strRoleName = tmpRol != null ? tmpRol.Name : "RECONECTOR";
            result = await UserManager.AddToRoleAsync(user, strRoleName);
            if (!result.Succeeded)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });
            }

            if (result.Succeeded)
            {
                try
                {
                    //Agrego el registro el tecnical staff
                    TechnicalStaff technicalStaff = new TechnicalStaff()
                    {
                        Name = addUser.Name,
                        Phone = addUser.Phone,
                        IsActive = addUser.IsActive,
                        OrderWorks = addUser.OrderWorks,
                        TechnicalRoleId = addUser.TechnicalRoleId,
                        TechnicalTeamId = addUser.TechnicalTeamId,
                        UserId = user.Id
                    };
                    _context.TechnicalStaffs.Add(technicalStaff);
                    _context.SaveChanges();

                    //Edito el registro en Phones, para saber a que usuario se asigno el telefono.                    
                    Phones tmpPhone = await _context.Phones.FirstOrDefaultAsync(p => p.PhoneNumber == addUser.Phone);
                    if(tmpPhone != null)
                    {
                        tmpPhone.AssignedUser = user.Id;
                        _context.Entry(tmpPhone).State = EntityState.Modified;                       
                        _context.SaveChanges();
                    }

                    return StatusCode(StatusCodes.Status200OK, new { msg = "Usuario creado con éxito Contraseña: " + password + " ,tambien se genero el usuario OT.", id = technicalStaff.Id, pass = password });
                }
                catch (Exception ex)
                {
                    return StatusCode((int)TypeError.Code.Ok, new { Error = "Usuario creado con éxito Contraseña: " + password + " ,No se genero el usuario de OT", id = 0, pass = password });
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> createRoleAsync(string name)
        {
            var roleExist = await RoleManager.RoleExistsAsync(name);

            if (!roleExist)
            {
                var roleResult = await RoleManager.CreateAsync(new ApplicationRol(name));
                if (roleResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)TypeError.Code.InternalServerError,
                                  new { Error = "Error al crear el role. Detalles del error: " + string.Join(" ", roleResult.Errors) });
                }
            }
            else
            {
                return StatusCode((int)TypeError.Code.Conflict,
                                  new { Error = "El rol ya existe favor de verificar" });
            }
        }

        [HttpPost("UpdateStaffOT/{id}")]
        public async Task<IActionResult> UpdateStaffOT([FromRoute] int id, [FromBody] TechnicalStaffVM user)
        {
            try
            {
                string password;
                TechnicalStaff technicalStaff;
                //Verifio si el usuario a actualizar ya tiene ligado un user
                if (user.UserId.Contains("nuevo"))
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        IdentityResult result;
                        var tmpDivision = await _context.Divisions.FirstOrDefaultAsync(d => d.Name.Contains("ORDEN DE TRABAJO"));
                        user.DivisionId = tmpDivision != null ? tmpDivision.Id : 2;
                        password = CrearPassword(6);
                        ApplicationUser AppUser = new ApplicationUser()
                        {
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = user.Nick,
                            Name = user.Name,
                            Email = user.Email,
                            PhoneNumber = user.Phone,
                            LastName = password,
                            SecondLastName = "OT",
                            DivitionId = user.DivisionId,
                            IsActive = user.IsActive
                        };
                        result = await UserManager.CreateAsync(AppUser, password);

                        if (!result.Succeeded)
                            return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });

                        var tmpRol = await _context.TechnicalRoles.FirstOrDefaultAsync(t => t.Id == user.TechnicalRoleId);
                        var tmpRolApp = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Contains(tmpRol.Name));
                        string strRoleName = tmpRolApp != null ? tmpRolApp.Name : "RECONECTOR";
                        result = await UserManager.AddToRoleAsync(AppUser, strRoleName);

                        if (!result.Succeeded)
                            return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) });

                        //Edito el registro del tecnical staff
                        technicalStaff = await _context.TechnicalStaffs.FirstOrDefaultAsync(t => t.Id == user.Id); //new TechnicalStaff()
                        technicalStaff.UserId = AppUser.Id;
                        technicalStaff.Phone = AppUser.PhoneNumber;
                        _context.TechnicalStaffs.Update(technicalStaff);
                        await _context.SaveChangesAsync();

                        //Edito el registro en Phones, para saber a que usuario se asigno el telefono.                    
                        Phones tmpPhone = await _context.Phones.FirstOrDefaultAsync(p => p.PhoneNumber == user.Phone);
                        if (tmpPhone != null)
                        {
                            tmpPhone.AssignedUser = AppUser.Id;
                            _context.Entry(tmpPhone).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }

                        scope.Complete();
                    }                    

                    return StatusCode(StatusCodes.Status200OK, new { msg = "Usuario creado con éxito Contraseña: " + password + " ,tambien se actualizó el usuario OT.", id = technicalStaff.Id, pass = password });                                           
                }
                else
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        //Verfico si cambio algo en el usuario
                        var userApp = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.UserId);
                        if (userApp != null && (userApp.UserName != user.Nick || userApp.Name != user.Name || userApp.Email != user.Email || userApp.PhoneNumber != user.Phone || userApp.DivitionId != user.DivisionId || userApp.IsActive != user.IsActive))
                        {
                            userApp.UserName = user.Nick;
                            userApp.Name = user.Name;
                            userApp.Email = user.Email;
                            userApp.PhoneNumber = user.Phone;
                            userApp.DivitionId = user.DivisionId;
                            userApp.IsActive = user.IsActive;
                            _context.Users.Update(userApp);
                            await _context.SaveChangesAsync();
                        }

                        //Verifico si cambio algun campo del userStaff
                        var staff = _context.TechnicalStaffs.Where(x => x.Id == id).First();
                        //Edito primero el registro en Phones si cambio.
                        if (staff != null && staff.Phone != user.Phone)
                        {
                            Phones oldPhone = await _context.Phones.FirstOrDefaultAsync(p => p.PhoneNumber == staff.Phone);
                            if(oldPhone != null)
                            {
                                oldPhone.AssignedUser = null;
                                _context.Entry(oldPhone).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }                            

                            Phones newPhone = await _context.Phones.FirstOrDefaultAsync(p => p.PhoneNumber == user.Phone);
                            if(newPhone != null)
                            {
                                newPhone.AssignedUser = user.UserId;
                                _context.Entry(newPhone).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }                            
                        }
                        //Edito el rol de usuario
                        if(staff != null && staff.TechnicalRoleId != user.TechnicalRoleId)
                        {
                            var tmpNewRol = await _context.TechnicalRoles.FirstOrDefaultAsync(t => t.Id == user.TechnicalRoleId);
                            var tmpNewRolApp = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Contains(tmpNewRol.Name));
                            string strNewRoleName = tmpNewRolApp != null ? tmpNewRolApp.Name : "RECONECTOR";
                            var newResult = await UserManager.AddToRoleAsync(userApp, strNewRoleName);

                            var tmpOldRol = await _context.TechnicalRoles.FirstOrDefaultAsync(t => t.Id == staff.TechnicalRoleId);
                            var tmpOldRolApp = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Contains(tmpOldRol.Name));
                            string strOldRoleName = tmpOldRolApp != null ? tmpOldRolApp.Name : "RECONECTOR";                            
                            var oldResult = await UserManager.RemoveFromRoleAsync(userApp, strOldRoleName);
                        }

                        //Edito el technicalStaff si tiene cambios.                
                        if (staff != null && (staff.Name != user.Name || staff.IsActive != user.IsActive || staff.Phone != user.Phone || staff.TechnicalRoleId != user.TechnicalRoleId || staff.TechnicalTeamId != user.TechnicalTeamId))
                        {
                            staff.Name = user.Name;
                            staff.IsActive = user.IsActive;
                            staff.Phone = user.Phone;
                            staff.TechnicalRoleId = user.TechnicalRoleId;
                            staff.TechnicalTeamId = user.TechnicalTeamId;
                            _context.TechnicalStaffs.Update(staff);
                            await _context.SaveChangesAsync();
                        }

                        scope.Complete();
                    }                    
                                       
                    return StatusCode(StatusCodes.Status200OK, new { msg = "Los datos se actualizaron correctamente" });
                }  
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }

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
            var res = _context.Users.Where(u => lstUsersInRol.Contains(u.Id) && u.IsActive == true && u.Serial != null).Select(x => new { Id = x.Id, Name = x.Name, LastName = x.LastName, SecondLastName = x.SecondLastName, divitionId = x.DivitionId }  ).ToList();
            return Ok(res);
        }

        [HttpGet("SerialFromUser/{pId}")]
        public async Task<IActionResult> GetSerialFromUser([FromRoute] string pId)
        {
            var res = _context.Users.Where(u => u.Id == pId && u.IsActive == true && u.Serial != null).FirstOrDefault().Serial;
            return Ok(res);
        }

        [HttpGet("teller")]
        public async Task<IActionResult> GetTeller()
        {
            var idRole = _context.Roles.Where(r => r.Name == "User").FirstOrDefault();
            var lstUsersInRol = _context.UserRoles.Where(ur => ur.RoleId == idRole.Id).Select(ur => ur.UserId).ToList();
            var res = _context.Users.Where(u => lstUsersInRol.Contains(u.Id) && u.IsActive == true).Select(x => new { Id = x.Id, Name = x.Name, LastName = x.LastName, SecondLastName = x.SecondLastName }).ToList();
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
            try
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
                            divitionName = divition?.Name,
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
            catch(Exception e)
            {
                return BadRequest(e);
            }
            
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
