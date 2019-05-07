using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.Api.Services.FirebaseService;
using Siscom.Agua.Api.Services.Security;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class DiscountAuthorizationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings appSettings;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly FirebaseDB firebaseDB = new FirebaseDB("https://siscom-notifications.firebaseio.com/");
        private FirebaseResponse response;
        public DiscountAuthorizationsController(ApplicationDbContext context, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.appSettings = appSettings.Value;
            this.userManager = userManager;
        }

        // GET: api/DiscountAuthorizations
        [HttpGet("List/{UserId}")]
        public async Task<IEnumerable<DiscountAuthorization>> GetDiscountAuthorizations([FromRoute] string UserId)
        {

            var data =  _context.DiscountAuthorizations
                                            .Include(x => x.DiscountAuthorizationDetails)
                                            .Where(x => x.UserRequestId == UserId).ToList();
            try
            {
                data.ForEach(x =>
                {
                    string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                    int start = name.Length - 4;
                    x.FileName = name.Remove(start, 4);
                    ApplicationUser FullName = userManager.FindByIdAsync(UserId).Result;
                    x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                });
            }
            catch (Exception ex)
            {

                throw;
            }

            return data;
        }

        [HttpGet("Firebase/{Key}")]
        public async Task<IActionResult> GetFirebase([FromRoute] string Key)
        {
            FirebaseDB firebaseDBNotificationsDiscount = firebaseDB.Node("DiscountAuthorization").Node(Key);
            response = firebaseDBNotificationsDiscount.Get();
            PushNotification notification = JsonConvert.DeserializeObject<PushNotification>(response.JSONContent);
            notification.ResponseDate = DateTime.Now.ToLocalTime();
            notification.IsReply = true;
            notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Revision);
            response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(notification));
            return Ok(notification);
        }
        // GET: api/DiscountAuthorizations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountAuthorization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discountAuthorization = await _context.DiscountAuthorizations.FindAsync(id);

            if (discountAuthorization == null)
            {
                return NotFound();
            }

            return Ok(discountAuthorization);
        }
        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload(IFormFile file)
        {
            //UploadFileLocal(file, )
            var data = Request.Form.Files;
            foreach (var fi in Request.Form.Files)
            {
                
            }
            var form = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscountAuthorization>(Request.Form["Data"].ToString());
            //var a = Request.Form.ke
            return Ok();
        }

        // PUT: api/DiscountAuthorizations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscountAuthorization([FromRoute] int id, [FromBody] DiscountAuthorization discountAuthorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != discountAuthorization.Id)
            {
                return BadRequest();
            }

            _context.Entry(discountAuthorization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountAuthorizationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DiscountAuthorizations
        [HttpPost]
        public async Task<IActionResult> PostDiscountAuthorization(IFormFile AttachedFile)
        {
            var MaxFileSize = (int)_context.SystemParameters.Where(n => n.Name == "FileMaxSize").FirstOrDefault().NumberColumn * 1024 * 1024;
            if (AttachedFile == null || AttachedFile.Length == 0)
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Archivo no seleccionado" });
            if (AttachedFile.Length > MaxFileSize)
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El archivo supero el tamaño maximo permitido" });

            var discountAuthorization = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscountAuthorization>(Request.Form["Data"].ToString());
            var discount = await _context.DiscountAuthorizations.Where(x => x.Account == discountAuthorization.Account).ToListAsync();

            if(discount.Count >= 1)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = $"La cuenta ya cuenta con una solicitud de descuento pendiente, por lo cual no se puede solicitar otro más hasta el dia: " });
            }

            String path = await UploadFileLocal(AttachedFile, discountAuthorization.Account, discountAuthorization.Folio);
            if(string.IsNullOrEmpty(path))
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para subir el archivo al servidor, vuleva a intentarlo" });
            discountAuthorization.FileName = path;
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _context.DiscountAuthorizations.Add(discountAuthorization);
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {

                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = Request.Form["Data"].ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para subir el archivo" });
            }
            

            return Ok(new
            {
               Id = discountAuthorization.Id,
               FileName = discountAuthorization.FileName
            });
        }

        // DELETE: api/DiscountAuthorizations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountAuthorization([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discountAuthorization = await _context.DiscountAuthorizations.FindAsync(id);
            if (discountAuthorization == null)
            {
                return NotFound();
            }

            _context.DiscountAuthorizations.Remove(discountAuthorization);
            await _context.SaveChangesAsync();

            return Ok(discountAuthorization);
        }

        private bool DiscountAuthorizationExists(int id)
        {
            return _context.DiscountAuthorizations.Any(e => e.Id == id);
        }
        
        private async Task<string> UploadFileLocal(IFormFile file, string Account, string Folio)
        {
            try
            {
                var uploadFilesPath = Path.Combine(appSettings.FilePath, "Descuentos", Account);

                if (!Directory.Exists(uploadFilesPath))
                    Directory.CreateDirectory(uploadFilesPath);

                //var fileName = CleanInput(file.FileName);
                var fileName = "Autorizacion_"+ Folio;
                var filePath = Path.Combine(uploadFilesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                GCHandle gch = GCHandle.Alloc(appSettings.IssuerExpedient, GCHandleType.Pinned);
                AESEncryption.FileEncrypt(filePath, appSettings.IssuerExpedient);
                AESEncryption.ZeroMemory(gch.AddrOfPinnedObject(), appSettings.IssuerExpedient.Length * 2);
                gch.Free();

                System.IO.File.Delete(filePath);
                FileInfo fileInfo = new FileInfo(filePath + ".aes");
                var newName = AESEncryptionString.EncryptString(file.FileName + ".aes", appSettings.IssuerName);
                while (newName.Contains("\\") || newName.Contains("/"))
                {
                    newName = AESEncryptionString.EncryptString(file.FileName + ".aes", appSettings.IssuerName);
                }
                fileInfo.Rename(newName);
                return fileInfo.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> ExectDiscount([FromRoute] int id, [FromBody] AuthorizationDiscountVM authorization)
        {
            DiscountAuthorization discount = await _context.DiscountAuthorizations.FindAsync(authorization.Id);

            if (discount.KeyFirebase != authorization.Key)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "la llave no coincide con la que está en la base de datos, favor de verificar" });
            }

            FirebaseDB firebaseDBNotificationsDiscount = firebaseDB.Node("DiscountAuthorization").Node(authorization.Key);
            response = firebaseDBNotificationsDiscount.Get();
            PushNotification @notification = JsonConvert.DeserializeObject<PushNotification>(response.JSONContent);

            if (authorization.Status == "EDE02")
            {
                foreach (var item in discount.DiscountAuthorizationDetails)
                {
                    string error = string.Empty;
                    try
                    {
                        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            using (var command = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = "billing_Adjusment";
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add(new SqlParameter("@id", item.DebtId != 0 ? item.DebtId : item.OrderSaleId));
                                if (discount.Type == "TDI01")
                                {
                                    command.Parameters.Add(new SqlParameter("@porcentage_value", 0));
                                    command.Parameters.Add(new SqlParameter("@discount_value", discount.AmountDiscount));
                                }
                                else if (discount.Type == "TDI02")
                                {
                                    command.Parameters.Add(new SqlParameter("@porcentage_value", discount.DiscountPercentage));
                                    command.Parameters.Add(new SqlParameter("@discount_value", 0));
                                }
                                command.Parameters.Add(new SqlParameter("@text_discount", authorization.ResponseObservations));
                                command.Parameters.Add(new SqlParameter("@option", item.DebtId != 0 ? 1 : 2));
                                command.Parameters.Add(new SqlParameter
                                {
                                    ParameterName = "@error",
                                    DbType = DbType.String,
                                    Size = 200,
                                    Direction = ParameterDirection.Output
                                });
                                this._context.Database.OpenConnection();
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    if (!result.HasRows)
                                    {
                                        error = command.Parameters["@error"].Value.ToString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(error))
                                {
                                    discount.ObservationResponse = authorization.ResponseObservations;
                                    discount.AuthorizationDate = DateTime.Now.ToLocalTime();
                                    discount.UserAuthorizationId = authorization.UserId;
                                    discount.Status = "EDE02";

                                    _context.Entry(discount).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    @notification.UserResponseId = authorization.UserId;
                                    @notification.ResponseDate = DateTime.Now.ToLocalTime();
                                    @notification.IsReply = true;
                                    @notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Autorizado);

                                    response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(notification));

                                    scope.Complete();
                                    return Ok();
                                }
                                else
                                {
                                    return StatusCode((int)TypeError.Code.Conflict, new { Error = error });
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        SystemLog systemLog = new SystemLog();
                        systemLog.Description = e.ToMessageAndCompleteStacktrace();
                        systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                        systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                        systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                        systemLog.Parameter = JsonConvert.SerializeObject(authorization);
                        CustomSystemLog helper = new CustomSystemLog(_context);
                        helper.AddLog(systemLog);
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para actualizar el contrato" });
                    }

                }
            }
            else if(authorization.Status == "EDE04")
            {
                discount.ObservationResponse = authorization.ResponseObservations;
                discount.AuthorizationDate = DateTime.Now.ToLocalTime();
                discount.UserAuthorizationId = authorization.UserId;
                discount.Status = authorization.Status;

                _context.Entry(discount).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                @notification.UserResponseId = authorization.UserId;
                @notification.ResponseDate = DateTime.Now.ToLocalTime();
                @notification.IsReply = true;
                @notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Autorizado);

                response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(notification));
            }
            
            return Ok();
        }
        private string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5)).Replace("-","");
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}