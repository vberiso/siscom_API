using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class SystemLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SystemLogController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("/InsertLog", Name = "insertlog")]
        public async Task<IActionResult> InsertLog(SystemLog log)
        {
            await _context.SystemLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecitar la transacción" });
        }
    }
}