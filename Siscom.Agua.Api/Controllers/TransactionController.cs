using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Siscom.Agua.Api.Controllers
{
    /// <summary>
    /// End Points Transaction
    /// </summary>
    [Route("api/Transaction")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of all Transaction
        /// </summary>
        /// <returns></returns>
        // GET: api/Transaction
        [HttpGet]
        public IEnumerable<DAL.Models.Transaction> GetTransaction()
        {
            return _context.Transactions;
        }

        /// <summary>
        /// This will provide details for the specific ID, of Transaction which is being passed
        /// </summary>
        /// <param name="id">Mandatory</param>
        /// <returns>Transaction Model</returns>
        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// This will provide capability add new Transaction
        /// </summary>       
        /// <param name="pConcepts">Model PaymentConcepts
        /// </param>
        /// <returns>New TerminalUser added</returns>
        // POST: api/Transaction
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] PaymentConceptsVM pConcepts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pConcepts))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }


            if (await _context.TerminalUsers.Where(x => x.Id == pConcepts.Transaction.TerminalUserId &&
                                                   x.InOperation == false)
                                            .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });
            }

            if (await _context.TerminalUsers.Where(x => x.Id == pConcepts.Transaction.TerminalUserId &&
                                                   x.OpenDate.Date != DateTime.Now.Date)
                                            .FirstOrDefaultAsync() != null)
            {
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });
            }

            DAL.Models.Transaction transaction = new DAL.Models.Transaction();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    transaction.DateTransaction = DateTime.Now;
                    transaction.Aplication = pConcepts.Transaction.Aplication;
                    transaction.Amount = pConcepts.Transaction.Amount;
                    transaction.Sign = pConcepts.Transaction.Sign;
                    transaction.TerminalUser = await _context.TerminalUsers.Include(x => x.Terminal).FirstOrDefaultAsync(y => y.Id == pConcepts.Transaction.TerminalUserId).ConfigureAwait(false);
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pConcepts.Transaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethod = await _context.PayMethods.FindAsync(pConcepts.Transaction.PayMethodId).ConfigureAwait(false);
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < pConcepts.Concepts.Count; i++)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pConcepts.Concepts[i].CodeConcept;
                        transactionDetail.amount = pConcepts.Concepts[i].amount;
                        transactionDetail.Description = pConcepts.Concepts[i].Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();
                    }

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    Folio folio = new Folio();
                    folio = await _context.Folios
                                          .Where(x => x.BranchOffice == transaction.TerminalUser.Terminal.BranchOffice &&
                                                       x.IsActive == 1).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                    TransactionFolio transactionFolio = new TransactionFolio();
                    transactionFolio.Folio = folio.Range + folio.BranchOffice.Id + "00" + folio.Secuential;
                    transactionFolio.DatePrint = DateTime.Now;
                    transactionFolio.Transaction = transaction;
                    _context.TransactionFolios.Add(transactionFolio);
                    await _context.SaveChangesAsync();

                    //folio.Secuential += 1;
                    //_context.Entry(folio).State = EntityState.Modified;
                    //await _context.SaveChangesAsync();
                    scope.Complete();

                }
                catch (DbUpdateException e)
                {
                    SystemLog systemLog = new SystemLog();
                    systemLog.Description = e.Message;
                    systemLog.DateLog = DateTime.Now;
                    systemLog.Controller = "TransactionController";
                    systemLog.Action = "PostTransaction";
                    systemLog.Parameter = JsonConvert.SerializeObject(pConcepts);
                    _context.SystemLogs.Add(systemLog);
                    _context.SaveChanges();

                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecitar la transacción" });
                }
                catch (System.Exception ex)
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = ex.Message });
                }
            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
            }
        }

        private bool Validate(PaymentConceptsVM pConcepts)
        {
            double sum = 0;

            if (pConcepts.Transaction.PayMethodId == 0)
                return false;
            if(pConcepts.Transaction.TerminalUserId == 0)
                return false;
            if (pConcepts.Transaction.TypeTransactionId == 0)
                return false;

            //Concept validation
            if (pConcepts.Concepts.Count() == 0)
                return false;            
            for (int i = 0; i < pConcepts.Concepts.Count(); i++)
            {
                sum += (double)pConcepts.Concepts[i].amount;
            }
            if (pConcepts.Transaction.Amount != sum)
                return false;

            return true;
        }
    }
}
