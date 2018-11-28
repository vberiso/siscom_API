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
        /// This will provide capability add new Transaction XXX
        /// </summary>       
        /// <param name="pConcepts">Model PaymentConcepts
        /// </param>
        /// <returns>New TerminalUser added</returns>
        // POST: api/Transaction
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] PaymentConceptsVM pConcepts)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
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

            if (!Validate(pConcepts))
            {
                return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("Información incompleta para realizar la transacción") });
            }

           
            if (pConcepts.Transaction.TypeTransactionId != 1 && pConcepts.Transaction.TypeTransactionId != 5)
            {
                if (!ValidConcept(pConcepts))
                {
                    return StatusCode((int)TypeError.Code.PartialContent, new { Error = string.Format("El detalle de conceptos no es correcto") });
                }
            }

            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Where(x => x.Id == pConcepts.Transaction.TerminalUserId).FirstOrDefaultAsync();

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
                        if (pConcepts.Transaction.TypeTransactionId == 1)
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha aperturado" });
                        _open = true;
                        break;
                    case 2://Fondo
                        if (pConcepts.Transaction.TypeTransactionId == 2)
                            return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha ingresado un fondo de caja" });                        
                        _fondoCaja = new KeyValuePair<int, Double>(_fondoCaja.Key + 1, item.Amount);
                        break;
                    case 3://Cobro
                        _cobrado = new KeyValuePair<int, Double>(_cobrado.Key + 1, item.Amount);
                        break;
                    case 4://Cancelado
                        _cancelado = new KeyValuePair<int, Double>(_cancelado.Key + 1, item.Amount);
                        break;
                    case 5://Cierre
                        _open = false;
                        break;
                    case 6: //Retiro
                        _retirado = new KeyValuePair<int, Double>(_retirado.Key + 1, item.Amount);
                        break;
                    case 7: //Liquidada
                        _liquidada = true;
                        break;
                    default:
                        break;
                }   
            }

            if (!_open && pConcepts.Transaction.TypeTransactionId == 1)
                _open = true;

            if (_open)
            {
                double _saldo=0;
                switch (pConcepts.Transaction.TypeTransactionId)
                {
                    case 1://apertura
                        pConcepts.Transaction.Amount = 0;
                        break;
                    case 2://Fondo
                        if (terminalUser.Terminal.CashBox > pConcepts.Transaction.Amount || pConcepts.Transaction.Amount==0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de fondo de caja inválido") });
                        break;
                    case 3://Cobro
                        if (_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La caja ya ha sido liquidada") });
                        if (!pConcepts.Transaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        break;
                    case 4://Cancelado
                        if (_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La caja ya ha sido liquidada") });
                        if (pConcepts.Transaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                        break;
                    case 5://Cierre
                        if (!_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La caja debe ser liquidada previamente") });
                        break;
                    case 6://Retiro
                        if (_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La caja ya ha sido liquidada") });
                        if (pConcepts.Transaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });

                        _saldo = _cobrado.Value - _cancelado.Value - _retirado.Value;
                        if (pConcepts.Transaction.Amount > _saldo)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto a retirar no es valido") });
                        break;
                    case 7:
                        if (_liquidada)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La caja ya ha sido liquidada") });
                        if (pConcepts.Transaction.Sign)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de liquidación incorrecta") });

                        _saldo = _fondoCaja.Value + _cobrado.Value - _cancelado.Value - _retirado.Value;
                        if (pConcepts.Transaction.Amount - _saldo != 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de liquidación no es valido") });
                        break;
                    default:
                        break;
                }

                var option = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromSeconds(60)
                };

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

                        if (pConcepts.Transaction.TypeTransactionId != 1  && pConcepts.Transaction.TypeTransactionId != 5)
                        {
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
                        }
                        await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                        if (pConcepts.Transaction.TypeTransactionId != 1 && pConcepts.Transaction.TypeTransactionId != 2 && pConcepts.Transaction.TypeTransactionId != 5)
                        {
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
                    systemLog.Parameter = JsonConvert.SerializeObject(pConcepts);
                    CustomSystemLog helper = new CustomSystemLog(_context);
                    helper.AddLog(systemLog);
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
                }

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

        private bool ValidConcept(PaymentConceptsVM pConcepts)
        {
            double sum = 0;
            
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
