using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
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
    [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
    [ApiController]
    [Authorize]
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
        // GET: api/Products/Division/5
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
        // GET: api/Products/Tariff/5
        [HttpGet("Tariff/{ProductId}")]
        public async Task<IActionResult> GetProductTariff([FromRoute]  int ProductId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tariff = await _context.TariffProducts
                                     .Include(x => x.Product)
                                       .ThenInclude(product => product.ProductParams)
                                     .Where(x => x.ProductId == ProductId &&
                                                  x.IsActive == 1).SingleOrDefaultAsync();

            if (tariff ==null)
                return NotFound();

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
        /// <param name="pDebt">Debt Model
        /// </param>
        /// <returns>products</returns>
        // POST: api/Products/Agreement/5
        [HttpPost("Agreement/{AgreementId}")]
        public async Task<IActionResult> PostProductAgreement([FromRoute]  int AgreementId, [FromBody]  Debt pDebt)
        {
            Agreement agreement = new Agreement();
            Debt debt = new Debt();

            #region Validación
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var param = await _context.SystemParameters
                                  .Where(x => x.Name == "DAYS_EXPIRE_ORDER").FirstOrDefaultAsync();

            if (param == null)
                return StatusCode((int)TypeError.Code.InternalServerError, new { Message = string.Format("No se encuenta parametro para cálculo de expiración") });

            if (AgreementId != 0)
            {
                //Agreement
                agreement = await _context.Agreements
                                          .Include(x => x.TypeIntake)
                                          .Include(x => x.TypeService)
                                          .Where(x => x.Id == AgreementId).FirstOrDefaultAsync();

                if (agreement == null)
                    return StatusCode((int)TypeError.Code.NotFound, new { Message = string.Format("El número de cuenta no existe") });

                if (agreement.TypeStateServiceId != 1)
                    return StatusCode((int)TypeError.Code.NotFound, new { Message = string.Format("El contrato no se encuentra activo") });

                //Deuda
                if (pDebt.Amount == 0)
                    return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format("Importe incorrecto") });

                if (pDebt.DebtDetails.Count == 0)
                    return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format("Debe ingresar conceptos a cobrar") });

                if (pDebt.Amount != pDebt.DebtDetails.Sum(x => x.Amount))
                    return StatusCode((int)TypeError.Code.Conflict, new { Message = string.Format("El detalle de la deuda no coincide con el total") });

                //Producto
                bool _validaProducto = true;
                Product _product = new Product();
                pDebt.DebtDetails.ToList().ForEach(x => {
                    _product = FindProduct(Convert.ToInt32(x.CodeConcept));
                    if (_product == null) _validaProducto = false;
                    if (!_product.IsActive) _validaProducto = false;
                });

                if (!_validaProducto)
                    return StatusCode((int)TypeError.Code.NotFound, new { Message = string.Format("No se encontró concepto o no se encuentra habilitado") });
               
            }
            else
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Message = string.Format("EndPoint No hablitado") });
            }
            #endregion

            try
            {
                var currentUserName = this.User.Claims.ToList()[1].Value;
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    debt.DebitDate = DateTime.UtcNow.ToLocalTime();
                    debt.FromDate = DateTime.UtcNow.ToLocalTime().Date;
                    debt.UntilDate = DateTime.UtcNow.ToLocalTime().Date;
                    debt.Derivatives = agreement.NumDerivatives;
                    debt.TypeIntake = agreement.TypeIntake.Name;
                    debt.TypeService = agreement.TypeService.Name;
                    debt.Consumption = "0";
                    debt.Amount = pDebt.Amount;
                    debt.OnAccount = 0;
                    debt.Year = Convert.ToInt16(DateTime.UtcNow.ToLocalTime().Year);
                    debt.Type = "TIP02";
                    debt.Status = "ED001";
                    debt.DebtPeriodId = 0;
                    debt.AgreementId = agreement.Id;
                    debt.ExpirationDate = DateTime.UtcNow.ToLocalTime().Date.AddDays(Convert.ToInt16(param.NumberColumn));
                    debt.DebtDetails = pDebt.DebtDetails;

                    _context.Debts.Add(debt);
                    await _context.SaveChangesAsync();

                    DebtStatus debtStatus = new DebtStatus();
                    debtStatus.Debt = debt;
                    debtStatus.DebtId = debt.Id;
                    debtStatus.id_status = debt.Status;
                    debtStatus.DebtStatusDate = debt.DebitDate;
                    debtStatus.User = currentUserName;

                    _context.DebtStatuses.Add(debtStatus);
                    _context.SaveChanges();
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
                systemLog.Parameter = JsonConvert.SerializeObject(pDebt);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }


            //RedirectToActionResult redirect = new RedirectToActionResult("GetOrderSaleById", "OrderSales", new { @id = debt.Id });
            //return redirect;

            return Ok(debt.Id);

        }


        [HttpGet("GetOrderSaleById/{id}")]
        public async Task<IActionResult> GetOrderSaleById(int id)
        {

            var orderSale = await _context.Debts
                                          .Include(x => x.DebtDetails)
                                          .Where(x => x.Id == id )
                                          .FirstOrDefaultAsync();

            if (orderSale == null)
            {
                return NotFound();
            }

            return Ok(orderSale);
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

        private Product FindProduct(int id)
        {            
            return _context.Products.Find(id); 
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