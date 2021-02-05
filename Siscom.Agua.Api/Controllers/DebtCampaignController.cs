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


        [HttpPost("CreateRecordFile/{FileName}/{TotalRecords}/{invitation}")]
        public async Task<IActionResult> CreateRecordFile([FromBody] object data,[FromRoute] string FileName = "", [FromRoute] int TotalRecords = 0, [FromRoute] bool invitation = true)
        {
            try
            {
                
                var file = new DebtCampaignFiles();
                var Data = JObject.Parse(JsonConvert.SerializeObject( data ));

                file.UserId = Data["UserId"].ToString();
                file.UserName = Data["UserName"].ToString();
                file.TotalRecords = TotalRecords;
                file.FileName = FileName;
                file.GenerationDate = DateTime.Now;
                file.IsInvitation = invitation;

                _context.DebtCampaignFiles.Add(file);
                await _context.SaveChangesAsync();
                file.IsInvitation = invitation;
                await _context.SaveChangesAsync();
                _context.Entry(file).Reload();
                return Ok(new { file.Folio, file.Id });
            }
            catch (Exception e)
            {
                return Ok(new {error= "Ocurrio yn error"});
            }

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
        [HttpPost("UploadFile/{Id}")]
        public async Task<IActionResult> UploadFile(IFormFile AttachedFile, [FromRoute] int Id)
        {

            try
            {

                var file = _context.DebtCampaignFiles.Where(x => x.Id == Id).FirstOrDefault();

                if (AttachedFile != null && file!= null)
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        await AttachedFile.CopyToAsync(memoryStream);
                        file.PDF = memoryStream.ToArray();


                    }
                    await _context.SaveChangesAsync();
                }



                return Ok();
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

        [HttpPost("fromAgreement")]
        public async Task<IActionResult> fromAgreement([FromBody] List<int> idsAgree)
        {
            try
            {
                var res = await _context.DebtCampaign.Where(x => idsAgree.Contains(x.AgreementId)).ToListAsync();
                return Ok(res);
            }
            catch (Exception e)
            {
                return Conflict(e);
            }
        }

        [HttpPost("GetAccountsCampaign")]
        public async Task<IActionResult> GetAccountsCampaign([FromBody] List<int> route)
        {
            var list = route.Select(x => x.ToString()).ToList();
            var query = await (from o in _context.DebtCampaign
                                                 .Include(x => x.Agreement)
                                                    .ThenInclude(x => x.Addresses)
                                                        .ThenInclude(s => s.Suburbs)
                                                 .Include(x => x.Agreement)
                                                    .ThenInclude(x => x.Clients)
                                                 .Include(x => x.Agreement)
                                                    .ThenInclude(x => x.TypeService)
                        where list.Contains(o.Ruta.ToString())
                        select o).ToListAsync();
            //_context.DebtCampaign.Where((x) => (x.Ruta.ToString().Contains(list)));
            return Ok(query);
        }

        [HttpPost("UpdateAccountDebtCampaign")]
        public async Task<IActionResult> UpdateAccountDebtCampaign([FromBody] List<int> accountIDs)
        {
            foreach (var item in accountIDs)
            {
                var original = _context.DebtCampaign.Find(item);
                original.Status = "ECD02";
                _context.Entry(original).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            //RedirectToActionResult redirect = new RedirectToActionResult("GetAccountsCampaignById", "DebtCampaign", new { @ids = accountIDs });
            return RedirectToAction("GetAccountsCampaignById", new { @ids = accountIDs });
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetAccountsCampaignById")]
        public async Task<IActionResult> GetAccountsCampaignById(List<int> ids)
        {
            List<DebtCampaign> debtCampaigns = new List<DebtCampaign>();
            ids.ForEach(y =>
            {
                var data = _context.DebtCampaign
                                    .Include(x => x.Agreement)
                                        .ThenInclude(x => x.Addresses)
                                            .ThenInclude(s => s.Suburbs)
                                    .Include(x => x.Agreement)
                                        .ThenInclude(x => x.Clients)
                                    .Include(x => x.Agreement)
                                        .ThenInclude(x => x.TypeService).Where(c => c.Id == y).FirstAsync();
                debtCampaigns.Add(data.Result);
            });
            
            return Ok(debtCampaigns);
        }
    }
}