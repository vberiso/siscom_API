using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxReceiptCancelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaxReceiptCancelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TaxReceiptCancels
        [HttpGet]
        public IEnumerable<TaxReceiptCancel> GetTaxReceiptCancels()
        {
            return _context.TaxReceiptCancels;
        }
    }
}