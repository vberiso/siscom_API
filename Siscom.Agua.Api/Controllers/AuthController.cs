using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;

        public AuthController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var env = appSettings.ValidAudience;
            string rolname = string.Empty;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return BadRequest(new { Error = "El usuario o contraseña son incorrectos favor de verificar!" });
            }
            else
            {
                if (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user))
                {
                    var LockEnd = await userManager.GetLockoutEndDateAsync(user);
                    return StatusCode(409, new { Error = string.Format("La cuenta se bloqueo temporalmente por seguridad. Intente dentro de {0} minutos", Math.Round((LockEnd.Value - DateTimeOffset.Now).TotalMinutes)) });
                }
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (userManager.SupportsUserLockout && await userManager.GetAccessFailedCountAsync(user) > 0)
                    {
                        await userManager.ResetAccessFailedCountAsync(user);
                    }

                    var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var item in await userManager.GetRolesAsync(user))
                    {
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, item));
                        rolname = item;
                    }

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.IssuerSigningKey));
                    var token = new JwtSecurityToken(
                            issuer: appSettings.ValidIssuer,
                            audience: appSettings.ValidAudience,
                            expires: DateTime.Now.AddHours(1),
                            claims: claims,
                            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                        );
                    return Ok(new
                    {
                        user = user.Id,
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        RolName = rolname
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
                            return StatusCode(409, new { Error = "Su cuenta ha sido bloqueada termporalmente, intente despues de 5 minutos" });
                        }
                        else
                        {
                            await userManager.AccessFailedAsync(user);
                            return BadRequest(new { Error = string.Format("Solo le quedan {0} antes de bloquar cuenta, favor de verificar", (5 - await userManager.GetAccessFailedCountAsync(user))) });
                        }
                    }
                }
            }
            return Unauthorized();
        }
    }
}