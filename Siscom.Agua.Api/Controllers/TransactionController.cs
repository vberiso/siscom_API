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
            DebtStatus status;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pPaymentConcepts.Transaction))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
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
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });
            }

          
            if (await _context.Transactions                             
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString() &&
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
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

                        if (cancelacion.Amount != pPaymentConcepts.Transaction.Amount)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de movimientos no coinciden") });

                        _validation = true;
                        break;    
                    default:
                        _validation = false;
                        break;
                }

                //Validación de montos a cuenta
                double sumDebt = 0;
                double sumDetail = 0;

                //pPaymentConcepts.Debt.ToList().ForEach(x =>
                //{
                //    sumDebt += x.OnAccount;

                //    x.DebtDetails.ToList().ForEach(y =>
                //    {
                //        sumDetail += y.OnAccount;
                //    });
                //    if (sumDetail != x.Amount)                   
                //        _validation = false;                   
                       
                //});

                //if(pPaymentConcepts.Transaction.Amount != sumDebt)
                //    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de movimientos no coinciden") });

                //if (!_validation)
                //    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de movimientos no coinciden") });

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
                            //Transacción en caja
                            transaction.Folio = Guid.NewGuid().ToString("D");
                            transaction.DateTransaction = DateTime.Now;
                            transaction.Sign = pPaymentConcepts.Transaction.Sign;
                            transaction.Amount = pPaymentConcepts.Transaction.Amount;
                            transaction.Aplication = pPaymentConcepts.Transaction.Aplication;
                            transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pPaymentConcepts.Transaction.TypeTransactionId).ConfigureAwait(false);
                            transaction.PayMethod = await _context.PayMethods.FindAsync(pPaymentConcepts.Transaction.PayMethodId).ConfigureAwait(false);
                            transaction.TerminalUser = terminalUser;
                            transaction.CancellationFolio = pPaymentConcepts.Transaction.Cancellation;
                            transaction.Tax = pPaymentConcepts.Transaction.Tax;
                            transaction.Rounding = pPaymentConcepts.Transaction.Rounding;
                            transaction.AuthorizationOriginPayment = pPaymentConcepts.Transaction.AuthorizationOriginPayment;
                            transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pPaymentConcepts.Transaction.ExternalOriginPaymentId).ConfigureAwait(false);
                            transaction.OriginPayment = await _context.OriginPayments.FindAsync(pPaymentConcepts.Transaction.OriginPaymentId).ConfigureAwait(false);
                            transaction.Total = pPaymentConcepts.Transaction.Total;
                            _context.Transactions.Add(transaction);
                            await _context.SaveChangesAsync();

                            await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                            foreach (var debt in pPaymentConcepts.Debt)
                            {
                                //Recibo a pagar
                                var deuda = await _context.Debts.FindAsync(debt.Id);

                                //PAGO
                                if (transaction.TypeTransaction.Id == 3)
                                {
                                    if (deuda.OnAccount  + debt.OnAccount> deuda.Amount)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta de deuda inválido") });

                                    deuda.Status = pPaymentConcepts.Transaction.DebtStatus;
                                    deuda.OnAccount = debt.OnAccount;
                                    _context.Entry(deuda).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    //Ingreso de status de recibo
                                    status = new DebtStatus()
                                    {
                                        id_status = deuda.Status,
                                        DebtStatusDate = transaction.DateTransaction,
                                        User = terminalUser.User.Name + ' ' + terminalUser.User.LastName,
                                        DebtId = debt.Id
                                    };
                                    _context.DebtStatuses.Add(status);
                                    await _context.SaveChangesAsync();

                                    //Inserta pago
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
                                        Type = pPaymentConcepts.Transaction.Type,
                                        OriginPayment = transaction.OriginPayment,
                                        PayMethod = transaction.PayMethod,
                                        TransactionFolio = transaction.Folio,
                                        Rounding = transaction.Rounding,
                                        ExternalOriginPayment = transaction.ExternalOriginPayment,
                                    });
                                    await _context.SaveChangesAsync();

                                }
                                else //Cancelación
                                {                                   
                                    if (deuda.OnAccount - debt.OnAccount < 0)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta de deuda inválido") });

                                    //Se obtiene estado anterior
                                    var statusDebt = await _context.DebtStatuses.Where(x => x.DebtId == debt.Id).OrderByDescending(x =>x.Id).ToListAsync();
                                    string statusAnterior = String.Empty;
                                    bool status_encontrado = false;
                                    int contador = 0;

                                    foreach (var item in statusDebt)
                                    {
                                        if (!status_encontrado)
                                        {
                                            if (contador ==0 && item.id_status != "ED005" && item.id_status != "ED004")
                                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La cancelación no procede. La deuda ha cambiado") });
                                            if (item.id_status != "ED005")
                                            {
                                                statusAnterior = item.id_status;
                                                status_encontrado = true;
                                            }
                                            contador += 1;
                                        }
                                        else
                                            break;
                                    }

                                    //se modifica estado de deuda
                                    deuda.Status = statusAnterior;
                                    deuda.OnAccount = deuda.OnAccount - debt.OnAccount;

                                    _context.Entry(deuda).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                   status = new DebtStatus()
                                    {
                                        id_status = deuda.Status,
                                        DebtStatusDate = transaction.DateTransaction,
                                        User = terminalUser.User.Name + ' ' + terminalUser.User.LastName,
                                        DebtId = debt.Id
                                    };
                                    _context.DebtStatuses.Add(status);
                                    await _context.SaveChangesAsync();

                                    //se modifica estado de pago
                                    var payment = await _context.Payments.Where(x => x.TransactionFolio == transaction.CancellationFolio &&
                                                                                     x.DebtId == debt.Id).FirstOrDefaultAsync();
                                    payment.Status = "EP002";
                                    _context.Entry(payment).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();
                                }

                                //Conceptos
                                foreach (var detail in debt.DebtDetails)
                                {
                                    //Ingro de detalle de transacción
                                    TransactionDetail transactionDetail = new TransactionDetail();
                                    transactionDetail.CodeConcept = detail.CodeConcept;
                                    transactionDetail.amount = detail.OnAccount;
                                    transactionDetail.Description = detail.NameConcept;
                                    transactionDetail.Transaction = transaction;
                                    _context.TransactionDetails.Add(transactionDetail);
                                    await _context.SaveChangesAsync();

                                    var conceptos = await _context.DebtDetails.Where(x => x.DebtId == debt.Id &&
                                                                                          x.Id == detail.Id).FirstOrDefaultAsync();

                                    if (transaction.TypeTransaction.Id == 3)
                                    {
                                        if(conceptos.OnAccount + detail.OnAccount > conceptos.Amount)
                                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                        conceptos.OnAccount = conceptos.OnAccount + detail.OnAccount;
                                    }
                                    else {
                                        if (conceptos.OnAccount- detail.OnAccount < 0)
                                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: conceptos.NameConcept) });
                                        conceptos.OnAccount = conceptos.OnAccount - detail.OnAccount;
                                    }                                        

                                    _context.Entry(conceptos).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();
                                }
                            }

                            //Toma folio
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
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });
            }
            else {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });
            }
            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        /// <summary>
        /// This will provide capability add new Transaction
        /// </summary>       
        /// <param name="AgreementId">Id Agreement
        /// <param name="pTransactionVM">Model TransactionVM
        /// </param>
        /// <returns>New TerminalUser added</returns>
        // POST: api/Transaction
        [HttpPost("Prepaid/{AgreementId}")]
        public async Task<IActionResult> PostTransactionPrepaid([FromRoute] int AgreementId, [FromBody] TransactionVM pTransactionVM)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            Prepaid prepaid;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validate(pTransactionVM))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == pTransactionVM.TerminalUserId).FirstOrDefaultAsync();

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
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });
            }


            if (await _context.Transactions
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString() &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() != null)

            {

                switch (pTransactionVM.TypeTransactionId)
                {
                    case 3://Cobro                      
                        if (!pTransactionVM.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        if (pTransactionVM.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto inválido") });

                        if (await _context.Debts.Include(dd => dd.DebtDetails)
                                          .Where(gs => _context.Statuses
                                          .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == AgreementId)
                                          .FirstOrDefaultAsync() != null)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Improcedente anticipo. La cuenta tiene adeudo") });

                        _validation = true;
                        break;
                    case 4://Cancelado                       
                        if (pTransactionVM.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });

                        if (pTransactionVM.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto inválido") });

                        if (String.IsNullOrEmpty(pTransactionVM.Cancellation))
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

                        var cancelacion = await _context.Transactions.Where(x => x.Folio == pTransactionVM.Cancellation).FirstAsync();
                        if (cancelacion == null)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

                        if (cancelacion.Amount != pTransactionVM.Amount)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de movimientos no coinciden") });                     

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
                            //Transacción en caja
                            transaction.Folio = Guid.NewGuid().ToString("D");
                            transaction.DateTransaction = DateTime.Now;
                            transaction.Sign = pTransactionVM.Sign;
                            transaction.Amount = pTransactionVM.Amount;
                            transaction.Aplication = pTransactionVM.Aplication;
                            transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pTransactionVM.TypeTransactionId).ConfigureAwait(false);
                            transaction.PayMethod = await _context.PayMethods.FindAsync(pTransactionVM.PayMethodId).ConfigureAwait(false);
                            transaction.TerminalUser = terminalUser;
                            transaction.CancellationFolio = pTransactionVM.Cancellation;
                            transaction.Tax = pTransactionVM.Tax;
                            transaction.Rounding = pTransactionVM.Rounding;
                            transaction.AuthorizationOriginPayment = pTransactionVM.AuthorizationOriginPayment;
                            transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pTransactionVM.ExternalOriginPaymentId).ConfigureAwait(false);
                            transaction.OriginPayment = await _context.OriginPayments.FindAsync(pTransactionVM.OriginPaymentId).ConfigureAwait(false);
                            transaction.Total = pTransactionVM.Total;
                            _context.Transactions.Add(transaction);
                            await _context.SaveChangesAsync();

                            await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                            TransactionDetail transactionDetail = new TransactionDetail();
                            transactionDetail.CodeConcept = "ANT01";
                            transactionDetail.amount = transaction.Amount;
                            transactionDetail.Description = "PAGO ANTICIPADO";
                            transactionDetail.Transaction = transaction;
                            _context.TransactionDetails.Add(transactionDetail);
                            await _context.SaveChangesAsync();

                            //PAGO
                            if (transaction.TypeTransaction.Id == 3)
                            {
                                prepaid = new Prepaid();
                                prepaid.PrepaidDate = transaction.DateTransaction;
                                prepaid.Amount = transaction.Amount;
                                prepaid.Accredited = 0;
                                prepaid.Status = "ANT01";
                                prepaid.AgreementId = AgreementId;
                                _context.Prepaids.Add(prepaid);
                                await _context.SaveChangesAsync();

                                //Inserta pago
                                await _context.Payments.AddAsync(new Payment
                                {
                                    PaymentDate = transaction.DateTransaction,
                                    BranchOffice = terminalUser.Terminal.BranchOffice.Name,
                                    Subtotal = transaction.Amount,
                                    PercentageTax = pTransactionVM.PercentageTax,
                                    Tax = transaction.Tax,
                                    Total = transaction.Amount + transaction.Tax + transaction.Rounding,
                                    AuthorizationOriginPayment = transaction.AuthorizationOriginPayment,
                                    DebtId = prepaid.Id,
                                    Status = "EP001",
                                    Type = pTransactionVM.Type,
                                    OriginPayment = transaction.OriginPayment,
                                    PayMethod = transaction.PayMethod,
                                    TransactionFolio = transaction.Folio,
                                    Rounding = transaction.Rounding,
                                    ExternalOriginPayment = transaction.ExternalOriginPayment,
                                });
                                await _context.SaveChangesAsync();

                               

                            }
                            else //Cancelación
                            {
                                //se modifica estado de pago
                                var payment = await _context.Payments.Where(x => x.TransactionFolio == transaction.CancellationFolio).FirstOrDefaultAsync();

                                prepaid = await _context.Prepaids.Where(x => x.Id == payment.DebtId).FirstAsync();

                                if (prepaid == null)
                                    return NotFound();
                                else {
                                    if(prepaid.Status != "ANT01")
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago no se puede cancelar porque el ya ha sido debengado el anticipo") });
                                }                           

                                payment.Status = "EP002";
                                _context.Entry(payment).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                                prepaid.Status = "ANT04";
                                _context.Entry(prepaid).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }

                            //Toma folio
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
                        systemLog.Parameter = JsonConvert.SerializeObject(pTransactionVM);
                        CustomSystemLog helper = new CustomSystemLog(_context);
                        helper.AddLog(systemLog);
                        return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                    }
                }
                else
                    return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });
            }
            else
            {
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });
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
        public async Task<IActionResult> PostTransactionCashBox([FromRoute] int teminalUserId, [FromBody] TransactionCashBoxVM pTransaction)
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
            
            if(pTransaction.TerminalUserId ==0 ||pTransaction.TypeTransactionId== 0)
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

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

            if (terminalUser.OpenDate.Date != DateTime.Now.Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });

            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                            x.TerminalUser.InOperation == true &&
                                                            x.DateTransaction.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString())
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
                        if (pTransaction.PayMethodId==0)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Falta método de pago" });
                        _validation = true;
                        break;
                    case 5://Cierre
                        if (!_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal debe ser liquidada previamente") });
                        pTransaction.Amount = 0;
                        _validation = true;
                        terminalUser.InOperation = false;
                        _context.Entry(terminalUser).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        break;
                    case 6://Retiro
                        if (_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal ya ha sido liquidada") });
                        if (pTransaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        if (pTransaction.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto inválido") });
                        _saldo = _cobrado.Value - _cancelado.Value - _retirado.Value;
                        if (pTransaction.PayMethodId == 0)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Especofocar método de pago" });
                        if (pTransaction.Amount > _saldo)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto a retirar no es valido") });
                        _validation = true;                       
                        break;
                    case 7://Liquidada
                        if (pTransaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de liquidación incorrecta") });
                        if (pTransaction.PayMethodId == 0)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Especificar método de pago" });
                        _saldo =( _fondoCaja.Value + _cobrado.Value) - _cancelado.Value - _retirado.Value;
                        if (pTransaction.Amount - _saldo != 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de liquidación no es valido") });
                        _validation = true;
                        break;
                    default:
                        _validation = false;
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Opción incorrecta" });                      
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
                            transaction.TerminalUser = terminalUser;
                            transaction.Tax =0;
                            transaction.Rounding = 0;
                            transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(1).ConfigureAwait(false);
                            transaction.OriginPayment = await _context.OriginPayments.FindAsync(1).ConfigureAwait(false);
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

        private bool Validate(TransactionVM ptransaction)
        {
            if (ptransaction.PayMethodId == 0)
                return false;
            if(ptransaction.TerminalUserId == 0)
                return false;
            if (ptransaction.TypeTransactionId == 0)
                return false;
            if (ptransaction.ExternalOriginPaymentId == 0)
                return false;
            if (ptransaction.OriginPaymentId == 0)
                return false;
            if (String.IsNullOrEmpty(ptransaction.DebtStatus) & ptransaction.Type !="PAY02")
                return false;
            return true;
        }
    }
}
