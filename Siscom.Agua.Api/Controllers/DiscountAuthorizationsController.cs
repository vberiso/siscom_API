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
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
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
        private readonly FirebaseDB firebaseDB;
        private FirebaseResponse response;
        public DiscountAuthorizationsController(ApplicationDbContext context, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.appSettings = appSettings.Value;
            this.userManager = userManager;
            firebaseDB = new FirebaseDB(_context.SystemParameters.Where(x => x.Name == "STRINGURLFIREBASE").Select(x => x.TextColumn).FirstOrDefault());
        }
       
        [HttpGet("GetPendindDiscountAuthorizationList")]
        public async Task<IEnumerable<DiscountAuthorizationVM>> GetListDiscountAuthorizations()
        {

            var data = _context.DiscountAuthorizations
                                            .Include(x => x.DiscountAuthorizationDetails)
                                            .Select(x => new DiscountAuthorizationVM()
                                            {
                                                Status = x.Status,
                                                FileName = x.FileName,
                                                FileNameDB = x.FileName,
                                                KeyFirebase = x.KeyFirebase,
                                                Folio = x.Folio,
                                                Account = x.Account,
                                                AccountAdjusted = x.AccountAdjusted,
                                                Amount = x.Amount,
                                                AmountDiscount = x.AmountDiscount,
                                                AuthorizationDate = x.AuthorizationDate,
                                                BranchOffice = x.BranchOffice,
                                                DiscountAuthorizationDetails = x.DiscountAuthorizationDetails,
                                                DiscountPercentage = x.DiscountPercentage,
                                                ExpirationDate = x.ExpirationDate,
                                                Id = x.Id,
                                                IdOrigin = x.IdOrigin,
                                                Observation = x.Observation,
                                                ObservationResponse = x.ObservationResponse,
                                                RequestDate = x.RequestDate,
                                                Type = x.Type,
                                                UserRequestId = x.UserRequestId,
                                                UserRequest = x.UserRequest,
                                                UserAuthorizationId = x.UserAuthorizationId,
                                            })
                                            .Where(x => x.Status == "EDE01" && x.ExpirationDate >= DateTime.Now.ToLocalTime()).ToList();
            try
            {
                if (appSettings.Local)
                {
                    data.ForEach(x =>
                    {
                        string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                        int start = name.Length - 4;
                        x.FileName = name.Remove(start, 4);
                        ApplicationUser FullName = userManager.FindByIdAsync(x.UserAuthorizationId).Result;
                        if (FullName != null)
                            x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                        ApplicationUser requestName = userManager.FindByIdAsync(x.UserRequestId).Result;
                        if(requestName != null)
                            x.NameUserRequest = $"{requestName.Name} {requestName.LastName} {requestName.SecondLastName}";

                        if (x.DiscountAuthorizationDetails.First().DebtId != 0)
                        {
                            var debt = _context.Debts.Include(dd => dd.DebtDetails)
                                                            .Where(gs => _context.Statuses
                                                                .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) &&
                                                                            gs.AgreementId == _context.Agreements.Where(p => p.Account == x.Account).Select(p => p.Id)
                                                                            .FirstOrDefault())
                                                                                .OrderBy(p => p.FromDate)
                                                                                .ToListAsync();

                            //var deb = debt.Result.Where(z => (z.Status == "ED005" || z.Status == "ED004")).ToList();
                            if (debt.Result.Count == 0)
                            {
                                x.IsApplied = true;
                            }
                            else
                            {
                                x.IsApplied = false;
                            }
                        }
                        else
                        {
                            if (_context.OrderSales.Where(z => z.Status == "EOS02" && z.Id == x.DiscountAuthorizationDetails.First().OrderSaleId).ToList().Count > 0)
                            {
                                x.IsApplied = true;
                            }
                            else
                            {
                                x.IsApplied = false;
                            }
                        }

                    });
                }
                else
                {
                    data.ForEach(x =>
                    {
                        string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                        x.FileName = name;
                        ApplicationUser FullName = userManager.FindByIdAsync(x.UserAuthorizationId).Result;
                        if (FullName != null)
                            x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                        ApplicationUser requestName = userManager.FindByIdAsync(x.UserRequestId).Result;
                        if (requestName != null)
                            x.NameUserRequest = $"{requestName.Name} {requestName.LastName} {requestName.SecondLastName}";
                        if (x.DiscountAuthorizationDetails.First().DebtId != 0)
                        {
                            var debt = _context.Debts.Include(dd => dd.DebtDetails)
                                                            .Where(gs => _context.Statuses
                                                                .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) &&
                                                                            gs.AgreementId == _context.Agreements.Where(p => p.Account == x.Account).Select(p => p.Id)
                                                                            .FirstOrDefault())
                                                                                .OrderBy(p => p.FromDate)
                                                                                .ToListAsync();

                            //var deb = debt.Result.Where(z => (z.Status == "ED005" || z.Status == "ED004")).ToList();
                            if (debt.Result.Count == 0)
                            {
                                x.IsApplied = true;
                            }
                            else
                            {
                                x.IsApplied = false;
                            }
                        }
                        else
                        {
                            if (_context.OrderSales.Where(z => z.Status == "EOS02" && z.Folio == x.AccountAdjusted).ToList().Count > 0)
                            {
                                x.IsApplied = true;
                            }
                            else
                            {
                                x.IsApplied = false;
                            }
                        }
                    });
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }

            return data;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ///  /// <remarks>
        /// Types request:
        ///
        /// string date example: 2019-05-14
        ///
        /// </remarks>
        /// 
        [HttpGet("GetAllDiscountAuthorizationList/{date}")]
        public async Task<IEnumerable<DiscountAuthorizationVM>> GetAllListDiscountAuthorizations([FromRoute]string date)
        {
            DateTime datee = new DateTime();
            DateTime.TryParse(date, out datee);
            if(datee != DateTime.MinValue)
            {
                var data = _context.DiscountAuthorizations
                                            .Include(x => x.DiscountAuthorizationDetails)
                                            .Select(x => new DiscountAuthorizationVM()
                                            {
                                                Status = x.Status,
                                                FileName = x.FileName,
                                                FileNameDB = x.FileName,
                                                KeyFirebase = x.KeyFirebase,
                                                Folio = x.Folio,
                                                Account = x.Account,
                                                AccountAdjusted = x.AccountAdjusted,
                                                Amount = x.Amount,
                                                AmountDiscount = x.AmountDiscount,
                                                AuthorizationDate = x.AuthorizationDate,
                                                BranchOffice = x.BranchOffice,
                                                DiscountAuthorizationDetails = x.DiscountAuthorizationDetails,
                                                DiscountPercentage = x.DiscountPercentage,
                                                ExpirationDate = x.ExpirationDate,
                                                Id = x.Id,
                                                IdOrigin = x.IdOrigin,
                                                Observation = x.Observation,
                                                ObservationResponse = x.ObservationResponse,
                                                RequestDate = x.RequestDate,
                                                Type = x.Type,
                                                UserRequestId = x.UserRequestId,
                                                UserRequest = x.UserRequest,
                                                UserAuthorizationId = x.UserAuthorizationId,
                                            })
                                            .Where(y => y.RequestDate.Date >= datee.Date)
                                            .OrderByDescending(x => x.RequestDate)
                                            .ToList();
                try
                {
                    if (appSettings.Local)
                    {
                        data.ForEach(x =>
                        {
                            string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                            int start = name.Length - 4;
                            x.FileName = name.Remove(start, 4);
                            ApplicationUser FullName = userManager.FindByIdAsync(x.UserAuthorizationId).Result;
                            if (FullName != null)
                                x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                            ApplicationUser requestName = userManager.FindByIdAsync(x.UserRequestId).Result;
                            x.NameUserRequest = $"{requestName.Name} {requestName.LastName} {requestName.SecondLastName}";
                            if (x.DiscountAuthorizationDetails.First().DebtId != 0)
                            {
                                var debt = _context.Debts.Include(dd => dd.DebtDetails)
                                                                .Where(gs => _context.Statuses
                                                                    .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) &&
                                                                                gs.AgreementId == _context.Agreements.Where(p => p.Account == x.Account).Select(p => p.Id)
                                                                                .FirstOrDefault())
                                                                                    .OrderBy(p => p.FromDate)
                                                                                    .ToListAsync();

                                //var deb = debt.Result.Where(z => (z.Status == "ED005" || z.Status == "ED004")).ToList();
                                if (debt.Result.Count == 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                            else
                            {
                                if (_context.OrderSales.Where(z => z.Status == "EOS02" && z.Id == x.DiscountAuthorizationDetails.First().OrderSaleId).ToList().Count > 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }

                        });
                    }
                    else
                    {
                        data.ForEach(x =>
                        {
                            string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                            x.FileName = name;
                            ApplicationUser FullName = userManager.FindByIdAsync(x.UserAuthorizationId).Result;
                            if (FullName != null)
                                x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                            ApplicationUser requestName = userManager.FindByIdAsync(x.UserRequestId).Result;
                            x.NameUserRequest = $"{requestName.Name} {requestName.LastName} {requestName.SecondLastName}";
                            if (x.DiscountAuthorizationDetails.First().DebtId != 0)
                            {
                                var debt = _context.Debts.Include(dd => dd.DebtDetails)
                                                                .Where(gs => _context.Statuses
                                                                    .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) &&
                                                                                gs.AgreementId == _context.Agreements.Where(p => p.Account == x.Account).Select(p => p.Id)
                                                                                .FirstOrDefault())
                                                                                    .OrderBy(p => p.FromDate)
                                                                                    .ToListAsync();

                                //var deb = debt.Result.Where(z => (z.Status == "ED005" || z.Status == "ED004")).ToList();
                                if (debt.Result.Count == 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                            else
                            {
                                if (_context.OrderSales.Where(z => z.Status == "EOS02" && z.Folio == x.AccountAdjusted).ToList().Count > 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

                return data;
            }
            else
            {
                return new List<DiscountAuthorizationVM>();
            }
        }

        // GET: api/DiscountAuthorizations
        [HttpGet("List/{UserId}/{date}")]
        public async Task<IEnumerable<DiscountAuthorizationVM>> GetDiscountAuthorizations([FromRoute] string UserId, [FromRoute] string date)
        {
            DateTime datee = new DateTime();
            DateTime.TryParse(date, out datee);

            var requests = _context.DiscountAuthorizations
                                    .Where(x => x.UserRequestId == UserId && 
                                                x.ExpirationDate < DateTime.Now.ToLocalTime() &&
                                                x.Status == "EDE01").ToList();
            requests.ForEach(x =>
            {
                x.Status = "EDE03";
                x.Observation = "LA_SOLICITUD_EXPIRO";
                _context.Entry(x).State = EntityState.Modified;
                _context.SaveChanges();
            });

            if (datee != DateTime.MinValue)
            {
                var data = _context.DiscountAuthorizations
                                           .Include(x => x.DiscountAuthorizationDetails)
                                           .Select(x => new DiscountAuthorizationVM()
                                           {
                                               Status = x.Status,
                                               FileName = x.FileName,
                                               KeyFirebase = x.KeyFirebase,
                                               Folio = x.Folio,
                                               Account = x.Account,
                                               AccountAdjusted = x.AccountAdjusted,
                                               Amount = x.Amount,
                                               AmountDiscount = x.AmountDiscount,
                                               AuthorizationDate = x.AuthorizationDate,
                                               BranchOffice = x.BranchOffice,
                                               DiscountAuthorizationDetails = x.DiscountAuthorizationDetails,
                                               DiscountPercentage = x.DiscountPercentage,
                                               ExpirationDate = x.ExpirationDate,
                                               Id = x.Id,
                                               IdOrigin = x.IdOrigin,
                                               Observation = x.Observation,
                                               ObservationResponse = x.ObservationResponse,
                                               RequestDate = x.RequestDate,
                                               Type = x.Type,
                                               UserRequestId = x.UserRequestId,
                                               UserRequest = x.UserRequest,
                                               UserAuthorizationId = x.UserAuthorizationId,
                                           })
                                           .Where(x => x.UserRequestId == UserId && x.RequestDate.Date >= datee.Date)
                                           .OrderByDescending(x => x.AuthorizationDate)
                                           .ToList();

                try
                {
                    if (appSettings.Local)
                    {
                        data.ForEach(x =>
                        {
                            string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                            int start = name.Length - 4;
                            x.FileName = name.Remove(start, 4);
                            ApplicationUser FullName = userManager.FindByIdAsync(x.UserAuthorizationId).Result;
                            if (FullName != null)
                                x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                            ApplicationUser requestName = userManager.FindByIdAsync(x.UserRequestId).Result;
                            x.NameUserRequest = $"{requestName.Name} {requestName.LastName} {requestName.SecondLastName}";
                            if(x.DiscountAuthorizationDetails.First().DebtId != 0)
                            {
                                var debt = _context.Debts.Include(dd => dd.DebtDetails)
                                                                .Where(gs => _context.Statuses
                                                                    .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) &&
                                                                                gs.AgreementId == _context.Agreements.Where(p => p.Account == x.Account).Select(p => p.Id)
                                                                                .FirstOrDefault())
                                                                                    .OrderBy(p => p.FromDate)
                                                                                    .ToListAsync();

                                //var deb = debt.Result.Where(z => (z.Status == "ED005" || z.Status == "ED004")).ToList();
                                if (debt.Result.Count == 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                            else
                            {
                                if(_context.OrderSales.Where(z => z.Status == "EOS02" && z.Id == x.DiscountAuthorizationDetails.First().OrderSaleId).ToList().Count > 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                           
                            //if(_context.Debts.Find(x.DiscountAuthorizationDetails.Any(x => x.DebtId)))
                        });
                    }
                    else
                    {
                        data.ForEach(x =>
                        {
                            string name = AESEncryptionString.DecryptString(x.FileName, appSettings.IssuerName);
                            x.FileName = name;
                            ApplicationUser FullName = userManager.FindByIdAsync(x.UserAuthorizationId).Result;
                            if (FullName != null)
                                x.NameUserResponse = $"{FullName.Name} {FullName.LastName} {FullName.SecondLastName}";
                            ApplicationUser requestName = userManager.FindByIdAsync(x.UserRequestId).Result;
                            x.NameUserRequest = $"{requestName.Name} {requestName.LastName} {requestName.SecondLastName}";
                            if (x.DiscountAuthorizationDetails.First().DebtId != 0)
                            {
                                var debt = _context.Debts.Include(dd => dd.DebtDetails)
                                                                .Where(gs => _context.Statuses
                                                                    .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && 
                                                                                gs.AgreementId == _context.Agreements.Where(p => p.Account == x.Account).Select(p => p.Id)
                                                                                .FirstOrDefault())
                                                                                    .OrderBy(p => p.FromDate)
                                                                                    .ToListAsync();

                                //var deb = debt.Result.Where(z => (z.Status == "ED005" || z.Status == "ED004")).ToList();
                                if (debt.Result.Count == 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                            else
                            {
                                if (_context.OrderSales.Where(z => z.Status == "EOS02" && z.Folio == x.AccountAdjusted).ToList().Count > 0)
                                {
                                    x.IsApplied = true;
                                }
                                else
                                {
                                    x.IsApplied = false;
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

                return data;
            }
            else
            {
                return new List<DiscountAuthorizationVM>();
            }  
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

        [HttpGet("GetDiscountAuthorizationByAccount/{Account}")]
        public async Task<IActionResult> GetDiscountAuthorizationByAccount([FromRoute] string Account)
        {
            return Ok(await _context.DiscountAuthorizations.Where(x => x.Account == Account &&
                                                                            x.Status == "EDE01" &&
                                                                            x.ExpirationDate >= DateTime.Now.ToLocalTime()).ToListAsync());
           
        }

        [HttpGet("DownloadFileAzure/{Account}/{FileName}")]
        public async Task<IActionResult> DownloadFileAzure([FromRoute] string Account, string FileName)
        {
            var replacename = FileName.Replace("[ ]", "+");
            string name = AESEncryptionString.DecryptString(replacename, appSettings.IssuerName);
            string removeencript = name.Remove(name.Length - 4, 4);

            if (appSettings.Local)
            {
                var path = Path.Combine(appSettings.FilePath, "Descuentos", Account, replacename);
                var pathd = Path.Combine(appSettings.FilePath, "Descuentos", Account, removeencript);
                if (!System.IO.File.Exists(path))
                {
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Archivo no se encuentra favor de verificar" });
                }
                else
                {
                    GCHandle gch = GCHandle.Alloc(appSettings.IssuerExpedient, GCHandleType.Pinned);
                    AESEncryption.FileDecrypt(path, pathd, appSettings.IssuerExpedient);
                    AESEncryption.ZeroMemory(gch.AddrOfPinnedObject(), appSettings.IssuerExpedient.Length * 2);
                    gch.Free();

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(pathd, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    string ext = System.IO.Path.GetExtension(pathd);
                    System.IO.File.Delete(pathd);
                    return File(memory, GetContentType(System.IO.Path.GetExtension(removeencript)), name.Remove(name.Length - 4, 4));
                }
            }
            else
            {
                CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials(appSettings.StorageDiscount, appSettings.DiscountKey), true);
                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                Account = Account.PadLeft(4, '0');
                CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(Account);
                CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(removeencript);
                try
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        await blockBlob.DownloadToStreamAsync(memStream).ConfigureAwait(false);
                        memStream.Position = 0;
                        return new FileContentResult(memStream.ToArray(), new MediaTypeHeaderValue(blockBlob.Properties.ContentType.ToString()))
                        {
                            FileDownloadName = removeencript
                        };
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        // POST: api/DiscountAuthorizations
        [HttpPost]
        public async Task<IActionResult> PostDiscountAuthorization(IFormFile AttachedFile)
        {
            String path = string.Empty;
            var MaxFileSize = (int)_context.SystemParameters.Where(n => n.Name == "FileMaxSize").FirstOrDefault().NumberColumn * 1024 * 1024;
            if (AttachedFile == null || AttachedFile.Length == 0)
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Archivo no seleccionado" });
            if (AttachedFile.Length > MaxFileSize)
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El archivo supero el tamaño maximo permitido" });

            var discountAuthorization = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscountAuthorization>(Request.Form["Data"].ToString());

                                                                       
            var discount = _context.DiscountAuthorizations.Where(x => x.Account == discountAuthorization.Account 
                                                                                && x.ExpirationDate > DateTime.Now.ToLocalTime() 
                                                                                && x.Status == "EDE01")
                                                                                .ToListAsync();

            var existDiscount = _context.DiscountAuthorizations.Where(x => x.Account == discountAuthorization.Account
                                                                                    && x.ExpirationDate < DateTime.Now.ToLocalTime()
                                                                                    && x.Status == "EDE01")
                                                                                    .ToListAsync();

            await Task.WhenAll(discount, existDiscount);

            FirebaseDB firebaseDBNotificationsDiscount;
            PushNotification @notification;

            if (existDiscount.Result.Count > 0)
            {
                existDiscount.Result.ForEach(x =>
                {
                    x.Status = "EDE03";
                    x.ObservationResponse = "LA SOLICITUD EXPIRO";
                    _context.Entry(x).State = EntityState.Modified;
                    _context.SaveChanges();

                    firebaseDBNotificationsDiscount = firebaseDB.Node("DiscountAuthorization").Node(x.KeyFirebase);
                    response = firebaseDBNotificationsDiscount.Get();
                    @notification = JsonConvert.DeserializeObject<PushNotification>(response.JSONContent);

                    notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Cancelado);

                    response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(@notification));
                });
            }

            if (discount.Result.Count >= 1)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = $"La cuenta ya cuenta con una solicitud de descuento pendiente, por lo cual no se puede solicitar otro más hasta el dia: {discount.Result.FirstOrDefault().ExpirationDate}" });
            }
            if (appSettings.Local)
            {
                path = await UploadFileLocal(AttachedFile, discountAuthorization.Account, discountAuthorization.Folio);
            }
            else
            {
                path = await UploadFileAzure(AttachedFile, discountAuthorization.Account, discountAuthorization.Folio);
            }
           
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

        [HttpPost("{id}")]
        public async Task<IActionResult> ExectDiscount([FromRoute] int id, [FromBody] AuthorizationDiscountVM authorization)
        {
            DiscountAuthorization discount = await _context.DiscountAuthorizations
                                                            .Include(x => x.DiscountAuthorizationDetails)
                                                            .Where(x => x.Id == authorization.Id)
                                                            .FirstOrDefaultAsync();
            decimal valueDiscount = 0;
            //decimal valuePercentage = Math.Round(Convert.ToDecimal(discount.DiscountPercentage) / discount.DiscountAuthorizationDetails.Count, 2);
            string error = string.Empty;
            string account = string.Empty;

            if (discount.KeyFirebase != authorization.Key)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "la llave no coincide con la que está en la base de datos, favor de verificar" });
            }

            FirebaseDB firebaseDBNotificationsDiscount = firebaseDB.Node("DiscountAuthorization").Node(authorization.Key);
            response = firebaseDBNotificationsDiscount.Get();
            PushNotification @notification = JsonConvert.DeserializeObject<PushNotification>(response.JSONContent);

            if (authorization.Status == "EDE02")
            {
                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var item in discount.DiscountAuthorizationDetails)
                        {
                            if (item.DebtId != 0)
                            {
                                var debt = await _context.Debts.FindAsync(item.DebtId);
                                valueDiscount = Math.Round((debt.Amount / discount.Amount) * discount.AmountDiscount, 2);
                            }
                            else
                            {
                                var order = await _context.OrderSales.FindAsync(item.OrderSaleId);
                                valueDiscount = Math.Round((order.Amount / discount.Amount) * discount.AmountDiscount, 2);
                            }

                            using (var command = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = "billing_Adjusment";
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add(new SqlParameter("@id", item.DebtId != 0 ? item.DebtId : item.OrderSaleId));

                                if (discount.Type == "TDI01")
                                {
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@porcentage_value",
                                        DbType = DbType.Int32,
                                        Value = 0
                                    });
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@discount_value",
                                        DbType = DbType.Decimal,
                                        Value = valueDiscount
                                    });
                                }
                                else if (discount.Type == "TDI02")
                                {
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@porcentage_value",
                                        DbType = DbType.Int32,
                                        Value = discount.DiscountPercentage
                                    });
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@discount_value",
                                        DbType = DbType.Decimal,
                                        Value = 0.0
                                    });
                                }
                                command.Parameters.Add(new SqlParameter
                                {
                                    ParameterName = "@text_discount",
                                    DbType = DbType.String,
                                    Size = 50,
                                    Value = authorization.ResponseObservations
                                }
                                );
                                command.Parameters.Add(new SqlParameter("@option", item.DebtId != 0 ? 1 : 2));

                                command.Parameters.Add(new SqlParameter
                                {
                                    ParameterName = "@account_folio",
                                    DbType = DbType.String,
                                    Size = 30,
                                    Direction = ParameterDirection.Output
                                });

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
                                    if (result.HasRows)
                                    {
                                        error = command.Parameters["@error"].Value.ToString();
                                    }

                                    account = command.Parameters["@account_folio"].Value.ToString();
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(error))
                        {
                            discount.ObservationResponse = authorization.ResponseObservations;
                            discount.AuthorizationDate = DateTime.Now.ToLocalTime();
                            discount.UserAuthorizationId = authorization.UserId;
                            discount.Status = authorization.Status;
                            discount.AccountAdjusted = account;

                            _context.Entry(discount).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            @notification.UserResponseId = authorization.UserId;
                            @notification.ResponseDate = DateTime.Now.ToLocalTime();
                            @notification.IsReply = true;
                            @notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Autorizado);
                            if(account.Contains("-"))
                                @notification.AccountAdjusted = account; 

                            response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(notification));
                            scope.Complete();
                        }
                        else
                        {
                            //return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format("No se pudo realizar el descuento favor de volver a intentarlo, si el problema continua favor de comunicarse con el administrador del sistema") });
                            return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format($"No se pudo realizar el descuento por las siguientes razones: [{error}]") });
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
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                }

                return Ok(account);
            }
            else if(authorization.Status == "EDE04")
            {
                discount.ObservationResponse = authorization.ResponseObservations;
                discount.AuthorizationDate = DateTime.Now.ToLocalTime();
                discount.UserAuthorizationId = authorization.UserId;
                discount.Status = authorization.Status;
                discount.AccountAdjusted = account;

                _context.Entry(discount).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                @notification.UserResponseId = authorization.UserId;
                @notification.ResponseDate = DateTime.Now.ToLocalTime();
                @notification.IsReply = false;
                @notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Rechazado);

                response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(notification));
                return Ok();
            }
            else if (authorization.Status == "EDE03")
            {
                discount.ObservationResponse = authorization.ResponseObservations;
                discount.AuthorizationDate = DateTime.Now.ToLocalTime();
                discount.UserAuthorizationId = authorization.UserId;
                discount.Status = authorization.Status;
                discount.AccountAdjusted = account;

                _context.Entry(discount).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                @notification.UserResponseId = authorization.UserId;
                @notification.ResponseDate = DateTime.Now.ToLocalTime();
                @notification.IsReply = false;
                @notification.Status = Enum.GetName(typeof(TypeStatus), TypeStatus.Cancelado);

                response = firebaseDBNotificationsDiscount.Put(JsonConvert.SerializeObject(notification));
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
            //return Ok();
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

        private async Task<string> UploadFileLocal(IFormFile file, string Account, string Folio)
        {
            try
            {
                var uploadFilesPath = Path.Combine(appSettings.FilePath, "Descuentos", Account);

                if (!Directory.Exists(uploadFilesPath))
                    Directory.CreateDirectory(uploadFilesPath);

                //var fileName = CleanInput(file.FileName);
                var fileName = string.Format("Solicitud_{0}{1}", Folio, System.IO.Path.GetExtension(file.FileName));
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
                var newName = AESEncryptionString.EncryptString(fileName + ".aes", appSettings.IssuerName);
                while (newName.Contains("\\") || newName.Contains("/"))
                {
                    newName = AESEncryptionString.EncryptString(fileName + ".aes", appSettings.IssuerName);
                }
                fileInfo.Rename(newName);
                return fileInfo.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task<string> UploadFileAzure(IFormFile file, string Account, string Folio)
        {
            try
            {
                string storageConnection = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", appSettings.StorageDiscount, appSettings.DiscountKey);
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(Account.PadLeft(4, '0').Replace("-", "").ToLower());

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
                cloudBlockBlob.Properties.ContentType = file.ContentType;
                await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());
                var newName = AESEncryptionString.EncryptString(file.FileName, appSettings.IssuerName);
                while (newName.Contains("\\") || newName.Contains("/"))
                {
                    newName = AESEncryptionString.EncryptString(file.FileName, appSettings.IssuerName);
                }
                return newName;
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = "SERVICE_AZURE";
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return "";
            }
        }

        private string GetContentType(string ext)
        {
            var types = GetMimeTypes();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        private bool DiscountAuthorizationExists(int id)
        {
            return _context.DiscountAuthorizations.Any(e => e.Id == id);
        }
    }
}