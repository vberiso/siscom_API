using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        //private readonly ApplicationDbContext _context;

        //public TransactionController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        ///// <summary>
        ///// Get list of all Transaction
        ///// </summary>
        ///// <returns></returns>
        //// GET: api/Transaction
        //[HttpGet]
        //public IEnumerable<Transaction> GetTransaction()
        //{
        //    return _context.Transactions;
        //}

        ///// <summary>
        ///// This will provide details for the specific ID, of Transaction which is being passed
        ///// </summary>
        ///// <param name="id">Mandatory</param>
        ///// <returns>Transaction Model</returns>
        //// GET: api/Transaction/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetTransaction([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var transaction = await _context.Transactions.FindAsync(id);

        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(transaction);
        //}

        ///// <summary>
        ///// This will provide capability add new Transaction
        ///// </summary>
        ///// <param name="ptransaction">Model TransactionVM</param>
        ///// <param name="pconcepts">Model PaymentConcepts
        ///// </param>
        ///// <returns>New TerminalUser added</returns>
        //// POST: api/Transaction
        //[HttpPost]
        //public async Task<IActionResult> PostTransaction([FromBody] TransactionVM ptransaction, [FromBody] PaymentConcepts pconcepts)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (!Validate(ptransaction, pconcepts))
        //    {
        //        return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
        //    }

        //    if (ptransaction.DateTransaction.Date != DateTime.Now.Date)
        //        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Fecha incorrecta" });


        //    if (await _context.TerminalUsers.Where(x => x.Id == ptransaction.TerminalUserId &&                                               
        //                                           x.InOperation == false)
        //                                    .FirstOrDefaultAsync() != null)
        //    {
        //        return StatusCode((int)TypeError.Code.Conflict, new { Error = "La terminal no se encuentra operando" });
        //    }

        //    Transaction transaction = new Transaction();
        //    transaction.DateTransaction = ptransaction.DateTransaction;
        //    transaction.Aplication = ptransaction.Aplication;
        //    transaction.Amount = ptransaction.Amount;
        //    transaction.Sign = ptransaction.Sign;
        //    transaction.TerminalUser = await _context.TerminalUsers.FindAsync(ptransaction.TerminalUserId);
        //    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(ptransaction.TypeTransactionId);
        //    transaction.PayMethod = await _context.PayMethods.FindAsync(ptransaction.PayMethodId);
        //    transaction.Folio = Guid.NewGuid().ToString("D");


        //    Folio folio = new Folio();

        //    _context.Transactions.Add(transaction);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        //}

        //private bool Validate(TransactionVM ptransaction, PaymentConcepts pconcepts)
        //{
        //    if (ptransaction.PayMethodId == 0)
        //        return false;
        //    if(ptransaction.TerminalUserId == 0)
        //        return false;
        //    if (ptransaction.TypeTransactionId == 0)
        //        return false;
        //    if (pconcepts.Concepts.Count == 0)
        //        return false;

        //    //double amount = 0;
        //    //for (int i = 0; i < pconcepts.Concepts.Count; i++)
        //    //{
        //    //    amount=

        //    //}
        //    return true;
        //}
    }
}
