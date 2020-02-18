using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CuentasContablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CuentasContablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderSales
        [HttpGet()]
        public async Task<IActionResult> GetCuentasContables()
        {
            var servicios = _context.Services
                    .Where(s => s.IsActive == true)
                    .Select(x => new TreeListItem() 
                    { 
                        Id = x.Id,
                        TipeService = "Service",
                        ParentId = 0, 
                        CodeConcept = x.Id.ToString(), 
                        Description = x.Name, 
                        AccountNumber = x.AccountNumber, 
                        IsActive = x.IsActive                        
                    })
                    .ToList();

            var productos = _context.Products
                    .Include(x => x.TariffProducts)
                    .Where(p => p.IsActive == true) // && p.TariffProducts.Count > 0)
                    .Select(x => new TreeListItem()
                    {
                        Id = x.Id,
                        TipeService = "Product",
                        ParentId = x.Parent,
                        CodeConcept = x.Id.ToString(),
                        Description = x.Name,
                        AccountNumber = x.TariffProducts.FirstOrDefault(t => t.IsActive == 1) == null ? "No editable" : x.TariffProducts.FirstOrDefault(t => t.IsActive == 1).AccountNumber,
                        IsActive = x.IsActive
                    })
                    .ToList();

            try
            {
                List<TreeListItem> lstItems = new List<TreeListItem>();
                lstItems.AddRange(servicios);
                lstItems.AddRange(productos);
                return Ok(lstItems);
            }
            catch (Exception ex)
            {
                //return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Problemas al consultar los registros {0}", ex.Message) });
            }
            return Ok();
        }

        [HttpPost("{id}")]        
        public async Task<IActionResult> PostCuentasContables([FromRoute] int id, [FromBody] TreeListItem treeListItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != treeListItem.Id)
            {
                return BadRequest();
            }

            try
            {
                if (treeListItem.TipeService.Contains("Servi"))
                {
                    var Servicio = _context.Services.Find(treeListItem.Id);
                    Servicio.AccountNumber = treeListItem.AccountNumber;
                    _context.Entry(Servicio).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var Producto = _context.TariffProducts.FirstOrDefault(t => t.ProductId == treeListItem.Id && t.IsActive == 1);
                    Producto.AccountNumber = treeListItem.AccountNumber;
                    _context.Entry(Producto).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {  
                return NotFound();
            }            
        }

        [HttpPost()]
        public async Task<IActionResult> PostNewCuentasContables([FromBody] TreeListItem treeListItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                        
            try
            {
                if (treeListItem.TipeService.Contains("Servi"))
                {
                    Siscom.Agua.DAL.Models.Service service = new DAL.Models.Service();
                    service.Name = treeListItem.Description;
                    service.Order = 2;
                    service.IsCommercial = false;
                    service.IsActive = true;
                    service.InAgreement = false;
                    service.AccountNumber = treeListItem.AccountNumber;

                    _context.Services.Add(service);                    
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //** No termine esta parte, porque aun no era requerida esta funcionalidad. **
                    //DAL.Models.Product product = new DAL.Models.Product();
                    //product.Name = treeListItem.Description;
                    //product.Order = 1;
                    //product.Parent = treeListItem.Id;
                    ////product.HaveTariff = true;
                    //product.IsActive = true;
                    ////product.HaveAccount = false;
                    //product.DivisionId = 1;


                    //await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

    }
}