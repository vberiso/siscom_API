using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.Api.Services.Security;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json", "application/json-patch+json", "multipart/form-data")]
    [ApiController]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly string[] ACCEPTED_FILE_TYPES = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", "docx", ".xls", "xlsx" };
        private readonly ApplicationDbContext _context;
        private readonly AppSettings appSettings;
        private UserManager<ApplicationUser> userManager;

        public FileUploadController(ApplicationDbContext context, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.appSettings = appSettings.Value;
            this.userManager = userManager;
        }


        [HttpPost("{AgreementId}/{TypeFile}/{description}"), DisableRequestSizeLimit]
        public async Task<IActionResult> FileUpload([FromRoute] int AgreementId, [FromRoute] string TypeFile, [FromRoute] string description, IFormFile file)
        {
            AgreementFile agreementFile = new AgreementFile();
            var currentUserName = this.User.Claims.ToList()[1].Value;
            var userId = this.User.Claims.ToList()[3].Value;
            try
            {
                var MaxFileSize = (int)_context.SystemParameters.Where(n => n.Name == "FileMaxSize").FirstOrDefault().NumberColumn * 1024 * 1024;
                var agreement = await _context.Agreements.FindAsync(AgreementId);
                if (file == null || file.Length == 0)
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Archivo no seleccionado" });
                if (file.Length > MaxFileSize)
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El archivo supero el tamaño maximo permitido" });
                if (!ACCEPTED_FILE_TYPES.Any(s => s == Path.GetExtension(file.FileName).ToLower()))
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Archivo no soportado favor de verificar" });

                var uploadFilesPath = Path.Combine(appSettings.FilePath, agreement.Account);

                if (!Directory.Exists(uploadFilesPath))
                    Directory.CreateDirectory(uploadFilesPath);
  
                var fileName = file.FileName;
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

                var fileSize = FileConverterSize.SizeSuffix(file.Length);
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    agreementFile.Name = fileInfo.Name;
                    agreementFile.IsActive = true;
                    agreementFile.Type = TypeFile;
                    agreementFile.extension = new FileInfo(file.FileName).Extension;
                    agreementFile.Size = Math.Round(Convert.ToDouble(fileSize.Split(' ')[0])) + " " + fileSize.Split(' ')[1];
                    agreementFile.UploadDate = DateTime.UtcNow.ToLocalTime();
                    agreementFile.UserId = userId;
                    agreementFile.User = await userManager.FindByIdAsync(userId);
                    agreementFile.AgreementId = agreement.Id;
                    agreementFile.Agreement = agreement;
                    agreementFile.Description = description;

                    await _context.AgreementFiles.AddAsync(agreementFile);
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
                systemLog.Parameter = string.Format("agreementId:{0}, typeFile:{1}, file:{2}", AgreementId, TypeFile, file.FileName);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para subir el archivo" });
            }



            return Ok(agreementFile);
        }
    }
}