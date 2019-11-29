using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Siscom.Agua.Api.Services.Settings;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
    public class PromotionsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;

        public PromotionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _context = context;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }
        // GET: api/Agreements
        [HttpGet("{GroupId}")]
        public async Task<IActionResult> Promotions([FromRoute] int GroupId)
        {
            try
            {
                var promotions = _context.Promotions.Where(x => x.PromotionGroupId == GroupId && x.IsActive).ToList();
                return Ok(promotions);
            }
            catch (Exception e)
            {
                return Conflict(new { error = e});
                
            }
        }

        [HttpPost("ApplyPromotionDebts/{PromotionId}/{user}/{userId}")]
        public async Task<IActionResult> Promotions([FromRoute] int PromotionId, [FromRoute] string user, [FromRoute] string userId, [FromBody] List<int> DebtsIds)
        {
            try
              {
                DebtsIds.ForEach(x =>
                {
                    var promotion = _context.PromotionDebt.Where(p => p.DebtId == x).FirstOrDefault();
                    if (promotion != null)
                    {                 
                            promotion.PromotionId = PromotionId;
                            _context.PromotionDebt.Update(promotion);

                    }
                    else
                    {
                        promotion = new PromotionDebt()
                        {
                            DebtApplyPromotion = DateTime.Now,
                            DebtId = x,
                            PromotionId = PromotionId,
                            user = user,
                            userId = userId
                        };
                        _context.PromotionDebt.Add(promotion);
                    }

                });
                _context.SaveChanges();

                return Ok(new { message = "promocion aplicada"});
            }
            catch (Exception ex)
            {
                return Conflict(new { error = ex});
            }
        }
    }
}