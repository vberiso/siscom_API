using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderSalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderSales
        [HttpGet]
        public IEnumerable<OrderSale> GetOrderSales()
        {
            return _context.OrderSales;
        }

        // GET: api/OrderSales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderSale([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderSale = await _context.OrderSales.FindAsync(id);

            if (orderSale == null)
            {
                return NotFound();
            }            

            return Ok(orderSale);
        }

        // GET: api/OrderSales/5
        [HttpGet("Folio/{folio}")]
        public async Task<IActionResult> GetOrderSaleFolio([FromRoute] string folio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if(string.IsNullOrEmpty(folio))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Folio incorrecto" });

            var orderSale = await _context.OrderSales
                                          .Include(x=> x.TaxUser)
                                            .ThenInclude(y => y.TaxAddresses)
                                          .Include(x => x.OrderSaleDetail)
                                          .Where(x => x.Folio == folio)
                                          .FirstOrDefaultAsync();

            orderSale.DescriptionStatus = await _context.Statuses
                                                        .Where(x => x.CodeName == orderSale.Status)
                                                        .Select(x => x.Description)
                                                        .FirstOrDefaultAsync();
            orderSale.DescriptionType = await _context.Types
                                                        .Where(x => x.CodeName == orderSale.Type)
                                                        .Select(x => x.Description)
                                                        .FirstOrDefaultAsync();

            if (orderSale == null)
            {
                return NotFound();
            }
         
            return Ok(orderSale);
        }

        // PUT: api/OrderSales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderSale([FromRoute] int id, [FromBody] OrderSale orderSale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderSale.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderSale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderSaleExists(id))
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

        // POST: api/OrderSales
        [HttpPost]
        public async Task<IActionResult> PostOrderSale([FromBody] OrderSale orderSale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrderSales.Add(orderSale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderSale", new { id = orderSale.Id }, orderSale);
        }

        // DELETE: api/OrderSales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderSale([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderSale = await _context.OrderSales.FindAsync(id);
            if (orderSale == null)
            {
                return NotFound();
            }

            _context.OrderSales.Remove(orderSale);
            await _context.SaveChangesAsync();

            return Ok(orderSale);
        }

        private bool OrderSaleExists(int id)
        {
            return _context.OrderSales.Any(e => e.Id == id);
        }
    }
}