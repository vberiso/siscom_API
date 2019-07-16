using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace Siscom.Agua.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    public class AuthController : Controller
    {
        //user manager
        private UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
            this._context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var env = appSettings.ValidAudience;
            //string rolname = string.Empty;
            var listRoles = new List<string>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ////Get all user with role Agente
            //var users = userManager.GetUsersInRoleAsync("Agente").Result;
            ////*******************************************

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return BadRequest(new { Error = "Usuario o contraseña incorrectos. Favor de verificar" });
            }
            else
            {
                if (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user))
                {
                    var LockEnd = await userManager.GetLockoutEndDateAsync(user);
                    return StatusCode(409, new { Error = string.Format("La cuenta se bloqueó temporalmente por seguridad. Intente dentro de {0} minutos", Math.Round((LockEnd.Value - DateTimeOffset.Now).TotalMinutes)) });
                }
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!user.IsActive)
                    {
                        return StatusCode(409, new { Error = "Su cuenta ha sido bloqueada permanentemente. Favor de comunicarse con el administrador" });
                    }

                    if (userManager.SupportsUserLockout && await userManager.GetAccessFailedCountAsync(user) > 0)
                    {
                        await userManager.ResetAccessFailedCountAsync(user);
                    }

                    var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.UniqueName, string.Format("{0} {1} {2}", user.Name, user.LastName, user.SecondLastName)),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    };

                    foreach (var item in await userManager.GetRolesAsync(user))
                    {
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, item));
                        //rolname = item;
                        listRoles.Add(item);
                    }

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.IssuerSigningKey));
                    var token = new JwtSecurityToken(
                            issuer: appSettings.ValidIssuer,
                            audience: appSettings.ValidAudience,
                            expires: DateTime.UtcNow.ToLocalTime().AddHours(9),
                            claims: claims,
                            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                        );
                    return Ok(new
                    {
                        user = user.Id,
                        fullName = string.Format("{0} {1} {2}", user.Name, user.LastName, user.SecondLastName),
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo.ToLocalTime(),
                        RolName = listRoles,
                        Divition = user.DivitionId,
                        CanStamp = user.CanStamp
                    });
                }
                else
                {
                    if (userManager.SupportsUserLockout && await userManager.GetLockoutEnabledAsync(user))
                    {

                        var contador = await userManager.GetAccessFailedCountAsync(user);
                        if (await userManager.GetAccessFailedCountAsync(user) >= 4)
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddMinutes(5)); //TODO: Datos configurables 
                            await userManager.ResetAccessFailedCountAsync(user);
                            return StatusCode(409, new { Error = "Su cuenta ha sido bloqueada termporalmente. Intente despues de 5 minutos" });
                        }
                        else
                        {
                            await userManager.AccessFailedAsync(user);
                            return BadRequest(new { Error = string.Format("Solo quedan {0} intentos antes de bloquear la cuenta", (5 - await userManager.GetAccessFailedCountAsync(user))) });
                        }
                    }
                }
                //if(await _context.Authorizations.Where(x => x.MAC == model.Address).SingleOrDefaultAsync() == null)
                //{
                //    return StatusCode((int)TypeError.Code.Unauthorized, new { Message = "Sin autorización para ingresar al sistema, favor de verificar " });
                //}
            }
            return Unauthorized();
        }

        [HttpPost("Logger")]
        public async Task<IActionResult> LoginLog([FromBody] LogginLog log)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.LogginLogs.Add(log);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}