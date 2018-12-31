using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{AgreementId}")]
        public async Task<IActionResult> GetNotofications([FromRoute] int AgreementId)
        {
            var notifications = await _context.Notifications
                                              .Include(x => x.NotificationDetails)
                                              .Where(i => i.AgreementId == AgreementId)
                                              .ToListAsync();

            return new ObjectResult(notifications);
        }

    }
}