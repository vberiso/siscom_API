using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siscom.Agua.DAL;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Siscom.Agua.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class DebtCampaignController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DebtCampaignController(ApplicationDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Get list of Accounts Apply for Campaign
        /// </summary>
        /// <returns></returns>
        // GET: api/DebtCampaign
        //[HttpPost("CreateRecordFile")]
        //public async Task<IActionResult> CreateRecordFile(IFormFile AttachedFile, [FromRoute] string FileName = "", [FromRoute] int TotalRecords = 0, [FromRoute] bool invitation = true, [FromRoute] string folio = "")
        //{


        //}
        [HttpPost("UploadFile/{FileName}/{TotalRecords}/{invitation}/{folio?}")]
        public async Task<IActionResult> UploadFile(IFormFile AttachedFile, [FromRoute] string FileName = "", [FromRoute] int TotalRecords = 0, [FromRoute] bool invitation = true, [FromRoute] string folio = "")
        {

            try
            {
                var file = new DebtCampaignFiles();
                var Data = JObject.Parse(Request.Form["Data"].ToString());
                if (AttachedFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        await AttachedFile.CopyToAsync(memoryStream);
                        file.PDF = memoryStream.ToArray();
                        file.UserId = Data["UserId"].ToString();
                        file.UserName = Data["UserName"].ToString();
                        file.TotalRecords = TotalRecords;
                        file.FileName = FileName;
                        file.GenerationDate = DateTime.Now;
                        file.Folio = folio;
                       
                    }
                    _context.DebtCampaignFiles.Add(file);
                    await _context.SaveChangesAsync();
                    file.IsInvitation = invitation;
                    _context.DebtCampaignFiles.Update(file);
                    await _context.SaveChangesAsync();
                    
                }
                return Ok(file.Folio);
            }
            catch (Exception e)
            {
                return Conflict(e);
            }

        }



        [HttpGet("GetFiles/{invitation}")]
        public async Task<IActionResult> GetFiles([FromRoute] bool invitation)
        {

            try
            {


                return Ok(_context.DebtCampaignFiles.Where(x => x.IsInvitation == invitation).ToList());
            }
            catch (Exception e)
            {
                return Conflict(e);
            }

        }

        [HttpGet("getFileById/{Id}")]
        public async Task<IActionResult> getFileById([FromRoute] int Id)
        {

            try
            {


                return Ok(_context.DebtCampaignFiles.Where(x => x.Id == Id).First());
            }
            catch (Exception e)
            {
                return Conflict(e);
            }

        }
        


    }
}