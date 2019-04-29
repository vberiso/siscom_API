﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Services.Extension;
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

        public DiscountAuthorizationsController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            this.appSettings = appSettings.Value;
        }

        // GET: api/DiscountAuthorizations
        [HttpGet]
        public IEnumerable<DiscountAuthorization> GetDiscountAuthorizations()
        {
            return _context.DiscountAuthorizations;
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

            String path = await UploadFileLocal(AttachedFile, discountAuthorization.Account);

            _context.DiscountAuthorizations.Add(discountAuthorization);
            //await _context.SaveChangesAsync();

            return Ok(discountAuthorization.Id);
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
        
        private async Task<string> UploadFileLocal(IFormFile file, string Account)
        {
            try
            {
                var uploadFilesPath = Path.Combine(appSettings.FilePath, "Descuentos", Account);

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
                return fileInfo.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}