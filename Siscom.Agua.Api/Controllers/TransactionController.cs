using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Helpers;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.Api.Services.Security;
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
        private readonly ConnectionString appSettings;

        public TransactionController(ApplicationDbContext context, IOptions<ConnectionString> appSettings)
        {
            _context = context;
            this.appSettings = appSettings.Value;
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
        /// <param name="pPaymentConcepts">Model PaymentConcepts
        /// </param>
        /// <returns>New TerminalUser added</returns>
        // POST: api/Transaction
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] PaymentConceptsVM pPaymentConcepts)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation=false;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pPaymentConcepts))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Where(x => x.Id == pPaymentConcepts.Transaction.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
            {
                return NotFound();
            }

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if (terminalUser.OpenDate.Date != DateTime.Now.Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });

            if (await _context.Transactions
                           .Include(x => x.TypeTransaction)
                           .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                       x.DateTransaction.Date == DateTime.Now.Date &&
                                       x.TypeTransaction.Id == 5 || x.TypeTransaction.Id == 7)
                           .FirstOrDefaultAsync() != null)

            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "El estado de la terminal no permite la transacción" });
            }

            var tmp = await _context.Transactions
                              .Include(x => x.TypeTransaction)
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date == DateTime.Now.Date &&
                                          x.TypeTransaction.Id == 1).FirstOrDefaultAsync();
            if (await _context.Transactions
                              .Include(x => x.TypeTransaction)
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date == DateTime.Now.Date &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() != null)

            {
             
                switch (pPaymentConcepts.Transaction.TypeTransactionId)
                {                    
                    case 3://Cobro                      
                        if (!pPaymentConcepts.Transaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        if (pPaymentConcepts.Transaction.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto inválido") });
                        _validation = true;
                        break;
                    case 4://Cancelado                       
                        if (pPaymentConcepts.Transaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        if (pPaymentConcepts.Transaction.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto inválido") });
                        if(String.IsNullOrEmpty(pPaymentConcepts.Transaction.Cancellation))
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación")});

                        var cancelacion = await _context.Transactions.Where(x => x.Folio == pPaymentConcepts.Transaction.Cancellation).FirstAsync();
                        if (cancelacion == null)
                        {
                            return NotFound();
                        }
                        if(cancelacion.Amount != pPaymentConcepts.Transaction.Amount)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = string.Format("Los montos de movimientos no coinciden") });
                        _validation = true;
                        break;    
                    default:
                        _validation = false;
                        break;
                }
                if (_validation)
                {
                    var option = new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromSeconds(60)
                    };

                    try
                    {
                        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            transaction.Folio = Guid.NewGuid().ToString("D");
                            transaction.DateTransaction = DateTime.Now;
                            transaction.Sign = pPaymentConcepts.Transaction.Sign;
                            transaction.Amount = pPaymentConcepts.Transaction.Amount;
                            transaction.Aplication = pPaymentConcepts.Transaction.Aplication;
                            transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pPaymentConcepts.Transaction.TypeTransactionId).ConfigureAwait(false);
                            transaction.PayMethod = await _context.PayMethods.FindAsync(pPaymentConcepts.Transaction.PayMethodId).ConfigureAwait(false);
                            transaction.TerminalUser = await _context.TerminalUsers.Include(x => x.Terminal).FirstOrDefaultAsync(y => y.Id == pPaymentConcepts.Transaction.TerminalUserId).ConfigureAwait(false);
                            transaction.CancellationFolio = pPaymentConcepts.Transaction.Cancellation;
                            transaction.Tax = pPaymentConcepts.Transaction.Tax;
                            transaction.Rounding = pPaymentConcepts.Transaction.Rounding;
                            transaction.AuthorizationOriginPayment = pPaymentConcepts.Transaction.AuthorizationOriginPayment;
                            transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pPaymentConcepts.Transaction.ExternalOriginPaymentId).ConfigureAwait(false);
                            transaction.OriginPayment = await _context.OriginPayments.FindAsync(pPaymentConcepts.Transaction.OriginPaymentId).ConfigureAwait(false);
                            _context.Transactions.Add(transaction);
                            await _context.SaveChangesAsync();

                            await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                            foreach (var debt in pPaymentConcepts.Debt)
                            {
                                var deuda = await _context.Debts.FindAsync(debt.Id);

                                if (transaction.TypeTransaction.Id == 3)
                                {
                                    deuda.Status = pPaymentConcepts.Transaction.Status;
                                    deuda.OnAccount = debt.OnAccount;
                                    _context.Entry(deuda).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    await _context.Payments.AddAsync(new Payment
                                    {
                                        PaymentDate = transaction.DateTransaction,
                                        BranchOffice = terminalUser.Terminal.BranchOffice.Name,
                                        Subtotal = transaction.Amount,
                                        PercentageTax = pPaymentConcepts.Transaction.PercentageTax,
                                        Tax = transaction.Tax,
                                        Total = transaction.Amount + transaction.Tax + transaction.Rounding,
                                        AuthorizationOriginPayment = transaction.AuthorizationOriginPayment,
                                        DebtId = debt.Id,
                                        Status = "EP001",
                                        Type = "",
                                        OriginPayment = transaction.OriginPayment,
                                        PayMethod = transaction.PayMethod,
                                        TransactionFolio = transaction.Folio,
                                        Rounding = transaction.Rounding,
                                        ExternalOriginPayment = transaction.ExternalOriginPayment,
                                    });
                                    await _context.SaveChangesAsync();

                                }
                                else
                                {
                                    deuda.Status = "ED001";
                                    deuda.OnAccount = 0;
                                    _context.Entry(deuda).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    var payment = await _context.Payments.Where(x => x.AuthorizationOriginPayment == transaction.Folio &&
                                                                                     x.DebtId == debt.Id).FirstOrDefaultAsync();
                                    payment.Status = "EP002";
                                    _context.Entry(payment).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();
                                }


                                foreach (var detail in debt.DebtDetails)
                                {
                                    TransactionDetail transactionDetail = new TransactionDetail();
                                    transactionDetail.CodeConcept = detail.ServiceId.ToString();
                                    transactionDetail.amount = detail.OnAccount;
                                    transactionDetail.Description = _context.Services.Find(detail.ServiceId).Name;
                                    transactionDetail.Transaction = transaction;
                                    _context.TransactionDetails.Add(transactionDetail);
                                    await _context.SaveChangesAsync();

                                    var conceptos = await _context.DebtDetails.Where(x => x.DebtId == debt.Id &&
                                                                                          x.ServiceId == detail.ServiceId).FirstOrDefaultAsync();
                                    conceptos.OnAccount = transaction.TypeTransaction.Id == 3 ? detail.OnAccount : 0;
                                    _context.Entry(conceptos).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();
                                }
                            }

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

                            folio.Secuential += 1;
                            _context.Entry(folio).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            scope.Complete();
                        }
                    }
                    catch (Exception e)
                    {
                        SystemLog systemLog = new SystemLog();
                        systemLog.Description = e.ToMessageAndCompleteStacktrace();
                        systemLog.DateLog = DateTime.Now;
                        systemLog.Controller = "TransactionController";
                        systemLog.Action = "PostTransaction";
                        systemLog.Parameter = JsonConvert.SerializeObject(pPaymentConcepts);
                        CustomSystemLog helper = new CustomSystemLog(_context);
                        helper.AddLog(systemLog);
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                    }
                }
                else
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Acción no permitida" });
            }
            else {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Debe aperturar una terminar para realizar una transacción" });
            }
            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        /// <summary>
        /// Create a cash box operation
        /// </summary>       
        /// /// <param name="teminalUserId">Model TransactionVM
        /// <param name="pTransaction">Model TransactionVM
        /// </param>
        /// <returns>New Transaction added</returns>
        // POST: api/Transaction
        [HttpPost("{teminalUserId}")]
        public async Task<IActionResult> PostTransactionCashBox([FromRoute] int teminalUserId, [FromBody] TransactionVM pTransaction)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            bool _open = false;
            bool _liquidada = false;
            KeyValuePair<int, double> _fondoCaja = new KeyValuePair<int, Double>(0, 0);
            KeyValuePair<int, double> _retirado = new KeyValuePair<int, Double>(0, 0);
            KeyValuePair<int, double> _cobrado = new KeyValuePair<int, Double>(0, 0);
            KeyValuePair<int, double> _cancelado = new KeyValuePair<int, Double>(0, 0);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }           

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Where(x => x.Id == teminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
            {
                return NotFound();
            }

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if(terminalUser.OpenDate.Date != DateTime.Now.Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });

            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                            x.DateTransaction.Date == DateTime.Now.Date)
                                                .OrderBy(x=> x.Id).ToListAsync();

            foreach (var item in movimientosCaja)
            {
                switch (item.TypeTransaction.Id)
                {
                    case 1://apertura
                        if (pTransaction.TypeTransactionId == 1)
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha aperturado" });
                        _open = true;
                        break;
                    case 2://Fondo
                        if (pTransaction.TypeTransactionId == 2)
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha ingresado un fondo de caja" });                        
                        _fondoCaja = new KeyValuePair<int, Double>(_fondoCaja.Key + 1, item.Amount);
                        break;                    
                    case 5://Cierre
                        _open = false;
                        if (pTransaction.TypeTransactionId == 5)
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha sido cerrada" });
                        break;
                    case 6: //Retiro
                        _retirado = new KeyValuePair<int, Double>(_retirado.Key + 1, item.Amount);
                        break;
                    case 7: //Liquidada
                        _liquidada = true;
                        if (pTransaction.TypeTransactionId == 7)
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha sido liquidada" });
                        break;
                    default:
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Opción incorrecta" });                       
                }   
            }

            if (!_open && pTransaction.TypeTransactionId == 1)
                _open = true;

            if (_open)
            {
                double _saldo=0;
                switch (pTransaction.TypeTransactionId)
                {
                    case 1://apertura
                        pTransaction.Amount = 0;
                        _validation = true;
                        break;
                    case 2://Fondo
                        if (terminalUser.Terminal.CashBox > pTransaction.Amount || pTransaction.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de fondo de terminal inválido") });
                        _validation = true;
                        break;
                    case 5://Cierre
                        if (!_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal debe ser liquidada previamente") });
                        pTransaction.Amount = 0;
                        _validation = true;
                        break;
                    case 6://Retiro
                        if (_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal ya ha sido liquidada") });
                        if (pTransaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        if (pTransaction.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto inválido") });
                        _saldo = _cobrado.Value - _cancelado.Value - _retirado.Value;
                        if (pTransaction.Amount > _saldo)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto a retirar no es valido") });
                        _validation = true;
                        break;
                    case 7://Liquidada
                        if (pTransaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de liquidación incorrecta") });

                        _saldo = _fondoCaja.Value + _cobrado.Value - _cancelado.Value - _retirado.Value;
                        if (pTransaction.Amount - _saldo != 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de liquidación no es valido") });
                        _validation = true;
                        break;
                    default:
                        _validation = false;
                        break;
                }

                if (_validation)
                {
                    var option = new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromSeconds(60)
                    };

                    try
                    {
                        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            transaction.Folio = Guid.NewGuid().ToString("D");
                            transaction.DateTransaction = DateTime.Now;
                            transaction.Sign = pTransaction.Sign;
                            transaction.Amount = pTransaction.Amount;
                            transaction.Aplication = pTransaction.Aplication;
                            transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pTransaction.TypeTransactionId).ConfigureAwait(false);
                            transaction.PayMethod = await _context.PayMethods.FindAsync(pTransaction.PayMethodId).ConfigureAwait(false);
                            transaction.TerminalUser = await _context.TerminalUsers.Include(x => x.Terminal).FirstOrDefaultAsync(y => y.Id == pTransaction.TerminalUserId).ConfigureAwait(false);
                            transaction.CancellationFolio = pTransaction.Cancellation;
                            transaction.Tax = pTransaction.Tax;
                            transaction.Rounding = pTransaction.Rounding;
                            transaction.AuthorizationOriginPayment = pTransaction.AuthorizationOriginPayment;
                            transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pTransaction.ExternalOriginPaymentId).ConfigureAwait(false);
                            transaction.OriginPayment = await _context.OriginPayments.FindAsync(pTransaction.OriginPaymentId).ConfigureAwait(false);
                            _context.Transactions.Add(transaction);
                            await _context.SaveChangesAsync();

                            if (pTransaction.TypeTransactionId == 2 || pTransaction.TypeTransactionId == 6 || pTransaction.TypeTransactionId == 7)
                            {
                                TransactionDetail transactionDetail = new TransactionDetail();
                                transactionDetail.CodeConcept = pTransaction.TypeTransactionId.ToString();
                                transactionDetail.amount = transaction.Amount;
                                transactionDetail.Description = _context.TypeTransactions.Find(pTransaction.TypeTransactionId).Name;
                                transactionDetail.Transaction = transaction;
                                _context.TransactionDetails.Add(transactionDetail);
                                await _context.SaveChangesAsync();
                            }
                            scope.Complete();
                        }
                    }
                    catch (Exception e)
                    {
                        SystemLog systemLog = new SystemLog();
                        systemLog.Description = e.ToMessageAndCompleteStacktrace();
                        systemLog.DateLog = DateTime.Now;
                        systemLog.Controller = "TransactionController";
                        systemLog.Action = "PostTransaction";
                        systemLog.Parameter = JsonConvert.SerializeObject(pTransaction);
                        CustomSystemLog helper = new CustomSystemLog(_context);
                        helper.AddLog(systemLog);
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                    }
                }
                else
                    return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Acción no permitida" });
            }
            else
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        /// <summary>
        /// Get all transactions of terminalUser from day
        /// </summary>
        /// <param name="date">date yyyy-mm-dd</param>
        /// <param name="terminalUserId">terminalUserId</param>
        /// <returns></returns>
        // GET: api/TerminalUser
        [HttpGet("{date}/{terminalUserId}")]
        public async Task<IActionResult> FindTransactions([FromRoute] string date, int terminalUserId)
        {
           var transaction = await _context.Transactions
                                    .Include(x => x.TypeTransaction)
                                    .Include(x => x.PayMethod)
                                    .Include(x=> x.TransactionFolios)
                                    .Where(x => x.TerminalUser.Id == terminalUserId &&
                                                x.DateTransaction.Date == Convert.ToDateTime(date).Date)
                                    .OrderBy(x => x.Id).ToListAsync();
           if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        private bool Validate(PaymentConceptsVM pConcepts)
        {
            if (pConcepts.Transaction.PayMethodId == 0)
                return false;
            if(pConcepts.Transaction.TerminalUserId == 0)
                return false;
            if (pConcepts.Transaction.TypeTransactionId == 0)
                return false;
            return true;
        }
    }
}
