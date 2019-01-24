using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// This will provide product by division
        /// </summary>       
        /// <param name="Id">id Product
        /// </param>
        /// <returns>products</returns>
        // GET: api/Products/Division/5
        [HttpGet("Child/{Id}")]
        public async Task<IActionResult> GetChild([FromRoute]  int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products
                                        .Where(x => x.Parent == Id &&
                                                    x.IsActive == true).ToListAsync();         

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);

        }

        /// <summary>
        /// This will provide product by division
        /// </summary>       
        /// <param name="DivisionId">id Division
        /// </param>
        /// <returns>products</returns>
        // POST: api/Products/Division/5
        [HttpGet("Division/{DivisionId}")]
        public async Task<IActionResult> GetProductDivision([FromRoute]  int DivisionId)
        {
            IEnumerable<Product> product;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (DivisionId == 0)
            {
                product = await _context.Products
                                        .Where(x => x.IsActive == true).ToListAsync();
            }
            else
            {
                product = await _context.Products
                                            .Where(x => x.DivisionId == DivisionId &&
                                                        x.IsActive == true).ToListAsync();
            }

            var childParent = Preorder(product);

            if (product == null)
            {
                return NotFound();
            }         

            return Ok(childParent);

        }

       

        private IEnumerable<Product>Preorder(IEnumerable<Product> list)
        {

            var nodesByParent = list.GroupBy(x => x.Parent)
                                    .ToDictionary(xs => xs.Key,
                                    xs => xs.OrderBy(x => x.Id).GetEnumerator());

            var stack = new Stack<IEnumerator<Product>>();
            var a = nodesByParent.FirstOrDefault().Key;
            stack.Push(nodesByParent[nodesByParent.FirstOrDefault().Key]);

            while (stack.Count > 0)
            {
                var nodes = stack.Peek();
                if (nodes.MoveNext())
                {
                    yield return nodes.Current;
                    IEnumerator<Product> children;
                    if (nodesByParent.TryGetValue(nodes.Current.Id, out children))
                        stack.Push(children);
                }
                else
                    stack.Pop();
            }
        }

        /// <summary>
        /// This will provide product by division
        /// </summary>       
        /// <param name="ProductId">id Product
        /// </param>
        /// <returns>products</returns>
        // POST: api/Products/Tariff/5
        [HttpGet("Tariff/{ProductId}")]
        public async Task<IActionResult> GetProductTariff([FromRoute]  int ProductId)
        {
           // TariffProductVM tariffProductVM = new TariffProductVM();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tariff = await _context.TariffProducts
                                        .Where(x => x.ProductId == ProductId &&
                                                    x.IsActive==1).SingleOrDefaultAsync();

            if(tariff ==null)
                return NotFound();


            ////var factor = await _context.SystemParameters
            ////                          .Where(x => x.Name == "FACTOR")
            ////                          .SingleOrDefaultAsync();


            ////var tax = await _context.SystemParameters
            ////                          .Where(x => x.Name == "IVA")
            ////                          .SingleOrDefaultAsync();


            ////tariffProductVM.IdProduct = tariff.ProductId;

            ////if (tariff.TimesFactor != 0)
            ////{
            ////    tariffProductVM.Type = (int)TypeTariffProduct.By.Factor;
            ////    tariffProductVM.Amount = tariff.TimesFactor * factor.NumberColumn;
            ////}
            ////else if (tariff.Percentage != 0)
            ////{
            ////    tariffProductVM.Type = (int)TypeTariffProduct.By.Percentage;
            ////}
            ////else if (tariff.IsVariable)
            ////{
            ////    tariffProductVM.Type = (int)TypeTariffProduct.By.Variable;
            ////}
            ////else
            ////{
            ////    tariffProductVM.Type = (int)TypeTariffProduct.By.Amount;
            ////    tariffProductVM.Amount = tariff.Amount;
            ////}

            ////if (tariff.HaveTax)
            ////{
            ////    tariffProductVM.Tax = (tariffProductVM.Amount * tax.NumberColumn) / 100; 
            ////}

            ////tariffProductVM.Total = tariffProductVM.Amount + tariffProductVM.Tax;

            return Ok(tariff);
        }


        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        /// <summary>
        /// This create product in Debt of Agreement
        /// </summary>    
        /// <param name="AgreementId">Id Agreement
        /// <param name="ProductVM">ProductVM Model
        /// </param>
        /// <returns>products</returns>
        // POST: api/Products/Division/5
        [HttpPost("Agreement/{AgreementId}")]
        public async Task<IActionResult> PostProductAgreement([FromRoute]  int AgreementId, [FromBody]  ProductVM ProductVM)
        {
            Agreement agreement = new Agreement();

            #region Validación
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (AgreementId != 0)
            {
                //Agreement
                if (ProductVM.Agreement.Id == 0)
                    return StatusCode((int)TypeError.Code.NotFound, new { Message = string.Format("Debe indicar un número de contrato") });

                agreement = await _context.Agreements.FindAsync(ProductVM.Agreement.Id);

                if (agreement == null)
                    return StatusCode((int)TypeError.Code.NotFound, new { Message = string.Format("El número de cuenta no existe") });

                if (agreement.TypeStateServiceId != 1)
                    return StatusCode((int)TypeError.Code.NotFound, new { Message = string.Format("El contrato no se encuentra activo") });

                //Deuda
                if (ProductVM.Debt.Amount != ProductVM.Debt.DebtDetails.Sum(x => x.Amount))
                    return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format("El Detalle de la deuda no coincide con el total") });
            }
            else
            {
                agreement.NumDerivatives = 0;
                //agreement.TypeIntake 
                return StatusCode((int)TypeError.Code.BadRequest, new { Message = string.Format("EndPoint No hablitado") });
            }
            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Debt debt = new Debt();
                    debt.DebitDate = DateTime.UtcNow;
                    debt.FromDate = DateTime.UtcNow.Date;
                    debt.UntilDate = DateTime.UtcNow.Date;
                    debt.Derivatives = agreement.NumDerivatives;
                    debt.Type = "TIP02";

                    _context.Debts.Add(ProductVM.Debt);
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "ProductController";
                systemLog.Action = "PostProductAgreement";
                systemLog.Parameter = JsonConvert.SerializeObject(ProductVM);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok(AgreementId);

        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private bool AgreementExists(int id)
        {
            return _context.Agreements.Any(e => e.Id == id);
        }
    }
}