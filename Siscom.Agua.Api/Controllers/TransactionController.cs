﻿using Microsoft.AspNetCore.Authorization;
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
using System.Data;
using System.Data.SqlClient;
using System.Web.Http.Cors;

namespace Siscom.Agua.Api.Controllers
{
    /// <summary>
    /// End Points Transaction
    /// </summary>
    [Route("api/Transaction")]
   // [EnableCors(origins: Model.Global.global, headers: "*", methods: "*")]
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

            TransactionPaymentVM transactionPayment = new TransactionPaymentVM();
            transactionPayment.Transaction =  await _context.Transactions
                                                            .Include(x => x.OriginPayment)
                                                            .Include(x => x.ExternalOriginPayment)
                                                            //.Include(x => x.PayMethod)
                                                            .Include(x => x.TerminalUser)
                                                                 .ThenInclude( y => y.Terminal)
                                                                       .ThenInclude( z => z.BranchOffice)
                                                            .Include(x => x.TransactionDetails)
                                                            .Include(x => x.TransactionFolios)
                                                            .Include(x => x.TypeTransaction)
                                                            .FirstOrDefaultAsync(a => a.Id == id);

            if (transactionPayment.Transaction == null)
            {
                return NotFound();
            }

            transactionPayment.Transaction.PayMethod = await _context.PayMethods.FindAsync(transactionPayment.Transaction.PayMethodId);

            transactionPayment.Payment = await _context.Payments
                                                        .Include(p => p.ExternalOriginPayment)
                                                        .Include(p => p.OriginPayment)
                                                        .Include(p => p.PayMethod)
                                                        .Include(p => p.PaymentDetails)
                                                        .Where(m => m.TransactionFolio == ((transactionPayment.Transaction.TypeTransaction.Id != 4) ? transactionPayment.Transaction.Folio : transactionPayment.Transaction.CancellationFolio))
                                                        .FirstOrDefaultAsync();

            if (transactionPayment.Payment != null)
            {

                transactionPayment.Payment.PaymentDetails.ToList().ForEach(x =>
               {
                   x.Debt = _context.Debts.Find(x.DebtId);
                   x.Prepaid = _context.Prepaids.Find(x.PrepaidId);
               });
            }

           

            return Ok(transactionPayment);
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
                                     .Include(x => x.TransactionFolios)
                                     .Where(x => x.TerminalUser.Id == terminalUserId)
                                     .OrderBy(x => x.Id).ToListAsync();
            transaction.ToList().ForEach(x =>
            {
                x.PayMethod = _context.PayMethods.Find(x.PayMethodId);
            });
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// Get state operation terminal user
        /// </summary>       
        /// <param name="teminalUserId">Model TransactionVM
        /// </param>
        /// <returns>State Operation Terminaluser</returns>
        // GET: api/Transaction
        [HttpGet("Terminal/{teminalUserId}")]
        public async Task<IActionResult> GetTransactionCashBox([FromRoute] int teminalUserId)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            int _typeTransaction = 0;

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


            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                            x.TerminalUser.InOperation == true)
                                                .OrderBy(x => x.Id).ToListAsync();

            foreach (var item in movimientosCaja)
            {
                switch (item.TypeTransaction.Id)
                {
                    case 1://apertura
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                    case 2://Fondo
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                    case 5://Cierre
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                    case 7: //Liquidada
                        _typeTransaction = item.TypeTransaction.Id;
                        break;
                }
            }

            return Ok(_typeTransaction);
        }

        /// <summary>
        /// Get all transactions of BranchOffice
        /// </summary>
        /// <param name="date">date yyyy-mm-dd</param>
        /// <param name="BranchOfficeId">BranchOfficeId</param>
        /// <returns></returns>
        // GET: api/Transaction
        [HttpGet("BranchOffice/{date}/{BranchOfficeId}")]
        public async Task<IActionResult> FindTransactionsAllBranchOffice([FromRoute] string date, int BranchOfficeId)
        {
            string error = string.Empty;
            List<TransactionBranchOfficeVM> entities = null;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "getTransactionBranchOffice";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@fecha", date));
                command.Parameters.Add(new SqlParameter("@branch", BranchOfficeId));

                this._context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(result);
                    entities = new List<TransactionBranchOfficeVM>();
                    foreach (DataRow item in dataTable.Rows)
                    {
                        var T = new TransactionBranchOfficeVM();
                        T.BranchOffice = item[0].ToString();
                        T.TerminalUserId = Convert.ToInt32(item[1].ToString());
                        T.Hour = item[2].ToString();
                        T.Sign = Convert.ToInt16(item[3]);
                        T.Amount = Convert.ToDecimal(item[4].ToString());
                        T.Tax = Convert.ToDecimal(item[5].ToString());
                        T.Rounding = Convert.ToDecimal(item[6].ToString());
                        T.Total = Convert.ToDecimal(item[7].ToString());
                        T.TypeTransaction = item[8].ToString();
                        T.PayMethod = item[9].ToString();
                        T.Origin_Payment = item[10].ToString();
                        T.External_Origin_Payment = item[11].ToString();
                        entities.Add(T);
                    }

                }
            }
            return Ok(entities);
        }

        [HttpGet("TransactionPaymentWithoutFactura/{date}/{BranchOfficeId}/{TypeTransactionId}")]
        public async Task<IActionResult> GetTransactionPaymentWithoutFactura([FromRoute] string date, int BranchOfficeId, int TypeTransactionId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TransactionPaymentWithoutFacturaVM transactionPayment = new TransactionPaymentWithoutFacturaVM();
            int tmpAño = int.Parse(date.Split("-")[0]);
            int tmpMes = int.Parse(date.Split("-")[1]);
            int tmpDia = int.Parse(date.Split("-")[2]);
            DateTime tmpFechaStart = new DateTime(tmpAño, tmpMes, tmpDia, 0, 0, 0);
            DateTime tmpFechaEnd = new DateTime(tmpAño, tmpMes, tmpDia, 23, 59, 59);

            //obtengo el listado de Transacciones por TypeTransaction y Fecha
            List<DAL.Models.Transaction> lstTransations = new List<DAL.Models.Transaction>();
            lstTransations = await _context.Transactions
                                            //.Include(x => x.OriginPayment)
                                            //.Include(x => x.ExternalOriginPayment)
                                            ////.Include(x => x.PayMethod)
                                            //.Include(x => x.TerminalUser)
                                            //     .ThenInclude(y => y.Terminal)
                                            //           .ThenInclude(z => z.BranchOffice)
                                            //.Include(x => x.TransactionDetails)
                                            .Include(x => x.TransactionFolios)
                                            //.Include(x => x.TypeTransaction)
                                            .Where(x => x.TypeTransactionId == TypeTransactionId && x.DateTransaction >= tmpFechaStart && x.DateTransaction <= tmpFechaEnd).ToListAsync();
                                            //.FirstOrDefaultAsync(a => a.Id == 10);
            if (lstTransations == null)
            {
                return NotFound();
            }

            //Obtengo la oficina
            var BranchOffice = _context.BranchOffices.Find(BranchOfficeId);
            if (BranchOffice == null)
            {
                return NotFound();
            }

            //Obtengo los id's de transactions para obtener los pagos segun esos id's.
            var lstIds = lstTransations.Select(t => t.Folio).ToList();
            List<Payment> lstPaymentRelacionadosATransacciones = new List<Payment>();
            lstPaymentRelacionadosATransacciones = await _context.Payments
                                                        //.Include(p => p.ExternalOriginPayment)
                                                        //.Include(p => p.OriginPayment)
                                                        //.Include(p => p.PayMethod)
                                                        .Include(p => p.PaymentDetails)
                                                        .Where(p => lstIds.Contains(p.TransactionFolio) && p.BranchOffice == BranchOffice.Name)
                                                        .ToListAsync();
                                                        //.Where(m => m.TransactionFolio == ((transactionPayment.Transaction.TypeTransaction.Id != 4) ? transactionPayment.Transaction.Folio : transactionPayment.Transaction.CancellationFolio))
                                                        //.FirstOrDefaultAsync();

            //Obtengo los id´s de Payments ya filtrado segun el BranchOffice
            var lstIdsBranch = lstPaymentRelacionadosATransacciones.Select(x => x.Id).ToList();
            var paymentsFacturados = _context.TaxReceipts.Where(x => lstIdsBranch.Contains(x.PaymentId) && x.Status == "ET001").Select(tr => tr.PaymentId).ToList();
            var paymentsFacturadosCancelados = _context.TaxReceipts.Where(x => lstIdsBranch.Contains(x.PaymentId) && x.Status == "ET002").Select(tr => tr.PaymentId).ToList();
            var paymentsFacturadosFinal = paymentsFacturados.Where(x => !paymentsFacturadosCancelados.Contains(x)).ToList();
            
            transactionPayment.lstPayment = lstPaymentRelacionadosATransacciones.Where(pp => !paymentsFacturadosFinal.Contains(pp.Id)).ToList();

            //Obtengo los id´s de folios en Payments para filtral los transactions.
            var lstIdsFoliosPayments = transactionPayment.lstPayment.Select(y => y.TransactionFolio).ToList();
            transactionPayment.lstTransaction = lstTransations.Where(yy => lstIdsFoliosPayments.Contains(yy.Folio)).ToList();

            if (transactionPayment.lstPayment == null)
            {
                return NotFound();
            }
                       
            return Ok(transactionPayment);
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
            Payment payment = new Payment();
            decimal _sumDebt = 0;
            decimal _sumDebtDetail = 0;
            decimal _sumPayDebtDetail = 0;
            decimal _sumTransactionDetail = 0;
            decimal _sumTaxDebtDetail = 0;
            bool _validation = true;

            #region Validación

           //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pPaymentConcepts.Transaction.TypeTransactionId != 3)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });

            if (!Validate(pPaymentConcepts.Transaction))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            if (!pPaymentConcepts.Transaction.Sign)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Naturaleza de transacción incorrecta"});

            if (await _context.Agreements
                             .Where(x => x.Account == pPaymentConcepts.Transaction.Account)
                             .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El número de cuenta no es correcto" });

            foreach (var item in pPaymentConcepts.Transaction.transactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if((pPaymentConcepts.Transaction.Amount + pPaymentConcepts.Transaction.Tax + pPaymentConcepts.Transaction.Rounding) != pPaymentConcepts.Transaction.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto"});


            if (pPaymentConcepts.Transaction.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción [{0}], no coincide con el total de la transacción [{1}]", _sumTransactionDetail, pPaymentConcepts.Transaction.Amount) });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == pPaymentConcepts.Transaction.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if (terminalUser.OpenDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });


            if (await _context.Transactions
                           .Include(x => x.TypeTransaction)
                           .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                       x.DateTransaction.Date == DateTime.UtcNow.ToLocalTime().Date &&
                                       (x.TypeTransaction.Id == 5 || x.TypeTransaction.Id == 7))
                           .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });


            if (await _context.Transactions
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.UtcNow.ToLocalTime().Date.ToShortDateString() &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });

            //Deuda
            pPaymentConcepts.Debt.ToList().ForEach(x =>
            {
                if (!String.IsNullOrEmpty(x.NewStatus))
                {
                    _sumDebtDetail = 0;
                    _sumPayDebtDetail = 0;                    
                    x.DebtDetails.ToList().ForEach(y =>
                    {
                        _sumPayDebtDetail += y.OnPayment;
                        _sumDebtDetail += y.OnAccount;

                        if (y.HaveTax)
                        {
                            _sumTaxDebtDetail += y.Tax;
                            if (y.Tax == 0)
                                _validation = false;                                
                        }
                    });
                    if (x.OnAccount != _sumDebtDetail)
                        _validation = false;
                    if (x.NewStatus == "ED005")
                    {
                        if (x.Amount != x.OnAccount)
                            _validation = false;
                        if (_sumDebtDetail != x.Amount)
                            _validation = false;
                    }
                    _sumDebt += _sumPayDebtDetail;
                }
            });

            if (!_validation)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos a pagar en el detalle no son correctos") });

            if (pPaymentConcepts.Transaction.Amount != _sumDebt)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de a pagar en el detalle no es correcto") });

            if(pPaymentConcepts.Transaction.Tax != _sumTaxDebtDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El IVA calculado no es correcto en su detalle ") });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pPaymentConcepts.Transaction.Sign;
                    transaction.Amount = pPaymentConcepts.Transaction.Amount;
                    transaction.Aplication = pPaymentConcepts.Transaction.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pPaymentConcepts.Transaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pPaymentConcepts.Transaction.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = pPaymentConcepts.Transaction.Cancellation;
                    transaction.Tax = pPaymentConcepts.Transaction.Tax;
                    transaction.Rounding = pPaymentConcepts.Transaction.Rounding;
                    transaction.AuthorizationOriginPayment = pPaymentConcepts.Transaction.AuthorizationOriginPayment;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pPaymentConcepts.Transaction.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(pPaymentConcepts.Transaction.OriginPaymentId).ConfigureAwait(false);
                    transaction.Total = pPaymentConcepts.Transaction.Total;
                    transaction.AccountNumber = pPaymentConcepts.Transaction.AccountNumber;
                    transaction.NumberBank = pPaymentConcepts.Transaction.NumberBank;
                    transaction.Account = pPaymentConcepts.Transaction.Account;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    foreach (var tDetail in pPaymentConcepts.Transaction.transactionDetails)
                    {
                        //Ingreso de detalle de transacción
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = tDetail.CodeConcept;
                        transactionDetail.Amount = tDetail.Amount;
                        transactionDetail.Description = tDetail.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();
                    }

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    //PAGOS                           
                    payment.PaymentDate = transaction.DateTransaction;
                    payment.BranchOffice = terminalUser.Terminal.BranchOffice.Name;
                    payment.Subtotal = transaction.Amount;
                    payment.PercentageTax = pPaymentConcepts.Transaction.PercentageTax;
                    payment.Tax = transaction.Tax;
                    payment.Rounding = Math.Truncate(transaction.Rounding * 100) / 100;
                    payment.Total = transaction.Total;
                    payment.AuthorizationOriginPayment = transaction.AuthorizationOriginPayment;
                    payment.NumberBank = transaction.NumberBank;
                    payment.AccountNumber = transaction.AccountNumber;
                    payment.AgreementId = pPaymentConcepts.Transaction.AgreementId;
                    payment.OrderSaleId = pPaymentConcepts.Transaction.OrderSaleId;
                    payment.Status = "EP001";
                    payment.Type = pPaymentConcepts.Transaction.Type;
                    payment.OriginPayment = transaction.OriginPayment;
                    payment.PayMethod = await _context.PayMethods.FindAsync(transaction.PayMethodId);
                    payment.TransactionFolio = transaction.Folio;
                    payment.ExternalOriginPayment = transaction.ExternalOriginPayment;
                    payment.Account = transaction.Account;                    
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    //Movimientos a deuda
                    foreach (var debt in pPaymentConcepts.Debt)
                    {
                        //Recibo a pagar
                        var debtFind = await _context.Debts.FindAsync(debt.Id);

                        if (!String.IsNullOrEmpty(debt.NewStatus))
                        {

                            if (await _context.Statuses
                                                      .Where(x => x.GroupStatusId == 4 &&
                                                                  x.CodeName == debtFind.Status).FirstAsync() != null)
                            {
                                debtFind.Status = debt.NewStatus;
                                debtFind.OnAccount = debt.OnAccount;
                                _context.Entry(debtFind).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                                DebtStatus debtStatus = new DebtStatus()
                                {
                                    id_status = debtFind.Status,
                                    DebtStatusDate = transaction.DateTransaction,
                                    User = terminalUser.User.Name + ' ' + terminalUser.User.LastName,
                                    DebtId = debt.Id
                                };
                                _context.DebtStatuses.Add(debtStatus);
                                await _context.SaveChangesAsync();

                                //Conceptos
                                foreach (var detail in debt.DebtDetails)
                                {
                                    var conceptos = await _context.DebtDetails.Where(x => x.DebtId == debt.Id &&
                                                                                          x.Id == detail.Id).FirstOrDefaultAsync();

                                    if (conceptos.OnAccount + detail.OnPayment != detail.OnAccount)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a pagar del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                    if (conceptos.OnAccount + detail.OnPayment > conceptos.Amount)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                    conceptos.OnAccount = conceptos.OnAccount + detail.OnPayment;
                                    _context.Entry(conceptos).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    string _accountNumber = String.Empty;
                                    string _unitMeasurement = String.Empty;

                                    if (debtFind.Type == "TIP01" || debtFind.Type == "TIP04")
                                    {
                                        var _serviceParam = await _context.ServiceParams
                                                                          .Where(x => x.ServiceId == Convert.ToInt32(!string.IsNullOrWhiteSpace(detail.CodeConcept) ? detail.CodeConcept : "0") && x.IsActive == true)
                                                                          .FirstOrDefaultAsync();

                                        _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                                        _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
                                    }
                                    else
                                    {
                                        var _productParam = await _context.ProductParams
                                                                          .Where(x => x.ProductId == Convert.ToInt32(!string.IsNullOrWhiteSpace(detail.CodeConcept) ? detail.CodeConcept : "0") && x.IsActive == true)
                                                                          .FirstOrDefaultAsync();
                                        _accountNumber = _productParam != null ? _productParam.CodeConcept : String.Empty;
                                        _unitMeasurement = _productParam != null ? _productParam.UnitMeasurement : String.Empty;
                                    }


                                    PaymentDetail paymentDetail = new PaymentDetail();
                                    paymentDetail.CodeConcept = detail.CodeConcept;
                                    paymentDetail.AccountNumber = _accountNumber;
                                    paymentDetail.UnitMeasurement = _unitMeasurement;
                                    paymentDetail.Amount = detail.OnPayment;
                                    paymentDetail.Description = detail.NameConcept;
                                    paymentDetail.DebtId = debt.Id;
                                    paymentDetail.PrepaidId = 0;
                                    paymentDetail.OrderSaleId = 0;
                                    paymentDetail.PaymentId = payment.Id;
                                    paymentDetail.HaveTax = detail.HaveTax;
                                    paymentDetail.Tax = detail.Tax;
                                    paymentDetail.Type = debt.Type;
                                    _context.PaymentDetails.Add(paymentDetail);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El estado - {0} de la deuda - {1}, no permite el pago", debtFind.Status, debtFind.Id) });
                        }
                    }

                    //Toma folio
                    //Folio folio = new Folio();
                    //folio = await _context.Folios
                    //                      .Where(x => x.BranchOffice == transaction.TerminalUser.Terminal.BranchOffice &&
                    //                                   x.IsActive == 1).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                    //TransactionFolio transactionFolio = new TransactionFolio();
                    //transactionFolio.Folio = folio.Range + folio.BranchOffice.Id + "00" + folio.Secuential;
                    //transactionFolio.DatePrint = DateTime.UtcNow.ToLocalTime();
                    //transactionFolio.Transaction = transaction;
                    //_context.TransactionFolios.Add(transactionFolio);
                    //await _context.SaveChangesAsync();

                    //folio.Secuential += 1;
                    //_context.Entry(folio).State = EntityState.Modified;
                    //await _context.SaveChangesAsync();

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pPaymentConcepts);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok(transaction.Id);
        }

        [HttpPost("OrderTransaction")]
        public async Task<IActionResult> PostOrdenTransaction([FromBody] PaymentOrdersVM pPaymentOrders)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            Payment payment = new Payment();
            decimal _sumOrder = 0;
            decimal _sumOrderDetail = 0;
            decimal _sumPayOrderDetail = 0;
            decimal _sumTransactionDetail = 0;
            decimal _sumTaxOrderDetail = 0;
            bool _validation = true;

            #region Validación

            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pPaymentOrders.Transaction.TypeTransactionId != 3)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });

            if (!Validate(pPaymentOrders.Transaction))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            if (!pPaymentOrders.Transaction.Sign)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Naturaleza de transacción incorrecta" });

            if( await _context.OrderSales
                              .Where(x=> x.Folio == pPaymentOrders.Transaction.Account)
                              .FirstOrDefaultAsync() ==null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El folio no existe" });

            foreach (var item in pPaymentOrders.Transaction.transactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((pPaymentOrders.Transaction.Amount + pPaymentOrders.Transaction.Tax + pPaymentOrders.Transaction.Rounding) != pPaymentOrders.Transaction.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });


            if (pPaymentOrders.Transaction.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, pPaymentOrders.Transaction.Amount) });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == pPaymentOrders.Transaction.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if (terminalUser.OpenDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });


            if (await _context.Transactions
                           .Include(x => x.TypeTransaction)
                           .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                       x.DateTransaction.Date == DateTime.UtcNow.ToLocalTime().Date &&
                                       (x.TypeTransaction.Id == 5 || x.TypeTransaction.Id == 7))
                           .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });


            if (await _context.Transactions
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.UtcNow.ToLocalTime().Date.ToShortDateString() &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });

            //Deuda
            pPaymentOrders.OrderSale.ToList().ForEach(x =>
            {
                if (!String.IsNullOrEmpty(x.Status))
                {
                    _sumOrderDetail = 0;
                    _sumPayOrderDetail = 0;
                    x.OrderSaleDetails.ToList().ForEach(y =>
                    {
                        _sumPayOrderDetail += y.OnAccount;
                        _sumOrderDetail += y.OnAccount;

                        if (y.HaveTax)
                        {
                            _sumTaxOrderDetail += y.Tax;
                            if (y.Tax == 0)
                                _validation = false;
                        }
                    });
                    if (x.OnAccount != _sumOrderDetail)
                        _validation = false;
                    if (x.Status == "EOS02")
                    {
                        if (x.Amount != x.OnAccount)
                            _validation = false;
                        if (_sumOrderDetail != x.Amount)
                            _validation = false;
                    }
                    _sumOrder += _sumPayOrderDetail;
                }
            });

            if (!_validation)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos a pagar en el detalle no son correctos") });

            if (pPaymentOrders.Transaction.Amount != _sumOrder)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de a pagar en el detalle no es correcto") });

            if (pPaymentOrders.Transaction.Tax != _sumTaxOrderDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El IVA calculado no es correcto en su detalle ") });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pPaymentOrders.Transaction.Sign;
                    transaction.Amount = pPaymentOrders.Transaction.Amount;
                    transaction.Aplication = pPaymentOrders.Transaction.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pPaymentOrders.Transaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pPaymentOrders.Transaction.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = pPaymentOrders.Transaction.Cancellation;
                    transaction.Tax = pPaymentOrders.Transaction.Tax;
                    transaction.Rounding = pPaymentOrders.Transaction.Rounding;
                    transaction.AuthorizationOriginPayment = pPaymentOrders.Transaction.AuthorizationOriginPayment;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pPaymentOrders.Transaction.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(pPaymentOrders.Transaction.OriginPaymentId).ConfigureAwait(false);
                    transaction.Total = pPaymentOrders.Transaction.Total;
                    transaction.AccountNumber = pPaymentOrders.Transaction.AccountNumber;
                    transaction.NumberBank = pPaymentOrders.Transaction.NumberBank;
                    transaction.Account = pPaymentOrders.Transaction.Account;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    foreach (var tDetail in pPaymentOrders.Transaction.transactionDetails)
                    {
                        //Ingreso de detalle de transacción
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = tDetail.CodeConcept;
                        transactionDetail.Amount = tDetail.Amount;
                        transactionDetail.Description = tDetail.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();
                    }
                    
                    //await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    //PAGOS                           
                    payment.PaymentDate = transaction.DateTransaction;
                    payment.BranchOffice = terminalUser.Terminal.BranchOffice.Name;
                    payment.Subtotal = transaction.Amount;
                    payment.PercentageTax = pPaymentOrders.Transaction.PercentageTax;
                    payment.Tax = transaction.Tax;
                    payment.Rounding = Math.Truncate(transaction.Rounding * 100) / 100;
                    payment.Total = transaction.Total;
                    payment.AuthorizationOriginPayment = transaction.AuthorizationOriginPayment;
                    payment.NumberBank = transaction.NumberBank;
                    payment.AccountNumber = transaction.AccountNumber;
                    payment.AgreementId = pPaymentOrders.Transaction.AgreementId;
                    payment.OrderSaleId = pPaymentOrders.Transaction.OrderSaleId;
                    payment.Status = "EP001";
                    payment.Type = pPaymentOrders.Transaction.Type;
                    payment.OriginPayment = transaction.OriginPayment;
                    payment.PayMethod = await _context.PayMethods.FindAsync(transaction.PayMethodId);
                    payment.TransactionFolio = transaction.Folio;
                    payment.ExternalOriginPayment = transaction.ExternalOriginPayment;
                    payment.Account = transaction.Account;
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    //Movimientos a deuda
                    foreach (var order in pPaymentOrders.OrderSale)
                    {
                        //Recibo a pagar
                        var orderFind = await _context.OrderSales.FindAsync(order.Id);

                        if (!String.IsNullOrEmpty(order.Status))
                        {

                            if (await _context.Statuses
                                                      .Where(x => x.GroupStatusId == 10 &&
                                                                  x.CodeName == orderFind.Status).FirstAsync() != null)
                            {
                                orderFind.Status = order.Status;
                                orderFind.OnAccount = order.OnAccount;
                                _context.Entry(orderFind).State = EntityState.Modified;
                                await _context.SaveChangesAsync();











                               //Conceptos
                                foreach (var detail in order.OrderSaleDetails)
                                {
                                    var conceptos = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == order.Id &&
                                                                                          x.Id == detail.Id).FirstOrDefaultAsync();
                                    
                                    //if (conceptos.OnAccount != detail.OnAccount)
                                    //    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a pagar del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                    if (conceptos.OnAccount > conceptos.Amount)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                    conceptos.OnAccount = detail.OnAccount;
                                    _context.Entry(conceptos).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    string _accountNumber = String.Empty;
                                    string _unitMeasurement = String.Empty;

                                    if (orderFind.Type == "OA001" || orderFind.Type == "OM001")
                                    {
                                        var _serviceParam = await _context.ServiceParams
                                                                          .Where(x => x.ServiceId == Convert.ToInt32(!string.IsNullOrWhiteSpace(detail.CodeConcept) ? detail.CodeConcept : "0") && x.IsActive == true)
                                                                          .FirstOrDefaultAsync();

                                        _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                                        _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
                                    }
                                    else
                                    {
                                        var _productParam = await _context.ProductParams
                                                                          .Where(x => x.ProductId == Convert.ToInt32(!string.IsNullOrWhiteSpace(detail.CodeConcept) ? detail.CodeConcept : "0") && x.IsActive == true)
                                                                          .FirstOrDefaultAsync();
                                        _accountNumber = _productParam != null ? _productParam.CodeConcept : String.Empty;
                                        _unitMeasurement = _productParam != null ? _productParam.UnitMeasurement : String.Empty;
                                    }


                                    PaymentDetail paymentDetail = new PaymentDetail();
                                    paymentDetail.CodeConcept = detail.CodeConcept;
                                    paymentDetail.AccountNumber = _accountNumber;
                                    paymentDetail.UnitMeasurement = _unitMeasurement;
                                    //paymentDetail.Amount = detail.OnPayment;
                                    paymentDetail.Description = detail.NameConcept;
                                    paymentDetail.DebtId = order.Id;
                                    paymentDetail.PrepaidId = 0;
                                    paymentDetail.OrderSaleId = 0;
                                    paymentDetail.PaymentId = payment.Id;
                                    paymentDetail.HaveTax = detail.HaveTax;
                                    paymentDetail.Tax = detail.Tax;
                                    paymentDetail.Type = order.Type;
                                    _context.PaymentDetails.Add(paymentDetail);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El estado: {0} de la deuda: {1}, no permite el pago", orderFind.Status, orderFind.Id) });
                        }
                    }

                    //Toma folio
                    //Folio folio = new Folio();
                    //folio = await _context.Folios
                    //                      .Where(x => x.BranchOffice == transaction.TerminalUser.Terminal.BranchOffice &&
                    //                                   x.IsActive == 1).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                    //TransactionFolio transactionFolio = new TransactionFolio();
                    //transactionFolio.Folio = folio.Range + folio.BranchOffice.Id + "00" + folio.Secuential;
                    //transactionFolio.DatePrint = DateTime.UtcNow.ToLocalTime();
                    //transactionFolio.Transaction = transaction;
                    //_context.TransactionFolios.Add(transactionFolio);
                    //await _context.SaveChangesAsync();

                    //folio.Secuential += 1;
                    //_context.Entry(folio).State = EntityState.Modified;
                    //await _context.SaveChangesAsync();

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pPaymentOrders);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok(transaction.Id);
        }

        /// <summary>
        /// This will provide capability add new Transaction
        /// </summary>       
        /// <param name="TransactionId">Id Transaction
        /// <param name="pCancelPayment">Model PaymentConcepts
        /// </param>
        /// <returns>Transaction</returns>
        // POST: api/Transaction/Cancel
        [HttpPost("Cancel/{TransactionId}")]
        public async Task<IActionResult> PostTransactionCancel([FromRoute] int TransactionId, [FromBody] CancelPaymentVM pCancelPayment)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            Payment payment = new Payment();
            decimal _sumTransactionDetail = 0;
            decimal sumPayDetail = 0;
            decimal _saldo = 0;

            #region Validación
            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pCancelPayment.Transaction.TypeTransactionId != 4)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });

            if (!Validate(pCancelPayment.Transaction))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            if (pCancelPayment.Transaction.Sign)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });

            if (pCancelPayment.Transaction.Amount <= 0)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cancelar incorrecto") });

            if (String.IsNullOrEmpty(pCancelPayment.Transaction.Cancellation))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

            foreach (var item in pCancelPayment.Transaction.transactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((pCancelPayment.Transaction.Amount + pCancelPayment.Transaction.Tax + pCancelPayment.Transaction.Rounding) != pCancelPayment.Transaction.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (pCancelPayment.Transaction.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción-> {0}, no coincide con el total de la transacción-> {1}", _sumTransactionDetail, pCancelPayment.Transaction.Amount) });

            if (pCancelPayment.Payment.PaymentDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No es posible cancelar pagos de otros días") });

            foreach (var item in pCancelPayment.Payment.PaymentDetails)
            {
                sumPayDetail += item.Amount;
            }
            if (pCancelPayment.Transaction.Amount != sumPayDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de detalle de pago no coinciden") });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == pCancelPayment.Transaction.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if (terminalUser.OpenDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });

            if (await _context.Transactions
                           .Include(x => x.TypeTransaction)
                           .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                       x.DateTransaction.Date == DateTime.UtcNow.ToLocalTime().Date &&
                                       (x.TypeTransaction.Id == 5 || x.TypeTransaction.Id == 7))
                           .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });


            if (await _context.Transactions
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString() &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });

            var movimientosCaja = await _context.Transactions
                                               .Include(x => x.TypeTransaction)
                                               .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                          x.PayMethodId == pCancelPayment.Transaction.PayMethodId &&
                                                          (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6 ))
                                               .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x => {              
                    _saldo += x.Sign ? x.Total : x.Total*-1;
            });


            if (pCancelPayment.Transaction.Amount > _saldo)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No hay fondos suficientes en caja para la devolución") });

            //Cancelación
            var cancelacion = await _context.Transactions.Where(x => x.Folio == pCancelPayment.Transaction.Cancellation).FirstAsync();
            if (cancelacion == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

            var cancelacionPrevia = await _context.Transactions
                                                  .Include(x => x.TransactionFolios)
                                                  .Where(x => x.CancellationFolio == pCancelPayment.Transaction.Cancellation).FirstOrDefaultAsync();
            if (cancelacionPrevia != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago ha sido cancelado previamente. Folio->{0}", cancelacionPrevia.TransactionFolios.FirstOrDefault().Folio) });

            if (cancelacion.Amount != pCancelPayment.Transaction.Amount)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de cancelación no coincide con el pago") });

            #endregion           

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pCancelPayment.Transaction.Sign;
                    transaction.Amount = pCancelPayment.Transaction.Amount;
                    transaction.Aplication = pCancelPayment.Transaction.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pCancelPayment.Transaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pCancelPayment.Transaction.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = pCancelPayment.Transaction.Cancellation;
                    transaction.Tax = pCancelPayment.Transaction.Tax;
                    transaction.Rounding = pCancelPayment.Transaction.Rounding;
                    transaction.AuthorizationOriginPayment = pCancelPayment.Transaction.AuthorizationOriginPayment;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pCancelPayment.Transaction.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(pCancelPayment.Transaction.OriginPaymentId).ConfigureAwait(false);
                    transaction.Total = pCancelPayment.Transaction.Total;
                    transaction.Account = pCancelPayment.Transaction.Account;
                    transaction.AccountNumber = pCancelPayment.Transaction.AccountNumber;
                    transaction.NumberBank = pCancelPayment.Transaction.NumberBank;                    
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    //se modifica estado de pago
                    payment = await _context.Payments.FindAsync(pCancelPayment.Payment.Id);
                    payment.Status = "EP002";
                    _context.Entry(payment).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var debtList = pCancelPayment.Payment.PaymentDetails.Select(x => x.DebtId).Distinct();

                    if (debtList != null)
                    {
                        foreach (var item in debtList)
                        {
                            Debt debt = new Debt();
                            debt = await _context.Debts.FindAsync(item);
                            decimal sumPaymentDetails = 0;

                            //Status anterior
                            var statusDebt = await _context.DebtStatuses.Where(x => x.DebtId == debt.Id).OrderByDescending(x => x.Id).ToListAsync();
                            string statusAnterior = String.Empty;

                            if (statusDebt != null)
                            {
                                if (statusDebt.Count >= 2)
                                {
                                    for (int i = 0; i < statusDebt.Count; i++)
                                    {
                                        if (i == 0)
                                        {
                                            if (statusDebt[i].id_status != "ED005" && statusDebt[i].id_status != "ED004")
                                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La cancelación no procede. La deuda ha cambiado") });
                                        }
                                        if (i == 1)
                                        {
                                            if (String.IsNullOrEmpty(statusDebt[i].id_status))
                                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se puede identificar estado de deuda. Comunicar con el administrador.") });
                                            statusAnterior = statusDebt[i].id_status;
                                            break;
                                        }

                                    }
                                }
                                else
                                    return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se puede identificar estado de deuda. Comunicar con el administrador.") });
                            }
                            else
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se puede identificar estado de deuda. Comunicar con el administrador.") });


                            pCancelPayment.Payment.PaymentDetails.ToList().ForEach(x =>
                            {
                                if (x.DebtId == item)
                                    sumPaymentDetails += x.Amount;
                            });

                            debt.Status = statusAnterior;
                            debt.OnAccount = debt.OnAccount - sumPaymentDetails;

                            _context.Entry(debt).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            DebtStatus debtStatus = new DebtStatus()
                            {
                                id_status = debt.Status,
                                DebtStatusDate = transaction.DateTransaction,
                                User = terminalUser.User.Name + ' ' + terminalUser.User.LastName,
                                DebtId = debt.Id
                            };
                            _context.DebtStatuses.Add(debtStatus);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No esposible revertir deuda sin detalle de pago") });


                    foreach (var pay in pCancelPayment.Payment.PaymentDetails)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pay.CodeConcept;
                        transactionDetail.Amount = pay.Amount;
                        transactionDetail.Description = pay.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();

                        DebtDetail debtDetail = new DebtDetail();
                        debtDetail = await _context.DebtDetails.Where(x => x.DebtId == pay.DebtId &&
                                                                           x.CodeConcept == pay.CodeConcept).FirstAsync();

                        if (debtDetail.OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        debtDetail.OnAccount -= pay.Amount;
                        await _context.SaveChangesAsync();
                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pCancelPayment);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }
            return Ok(transaction.Id);
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
            Prepaid prepaid;
            decimal _sumTransactionDetail = 0;
            string _accountNumber = String.Empty;
            string _unitMeasurement = String.Empty;

            #region Validación

            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pTransactionVM.TypeTransactionId != 3)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });

            if (!Validate(pTransactionVM))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            if (!pTransactionVM.Sign)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });

            if (await _context.Agreements
                              .Where(x => x.Account == pTransactionVM.Account)
                              .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El número de cuenta no es correcto" });

            foreach (var item in pTransactionVM.transactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((pTransactionVM.Amount + pTransactionVM.Tax + pTransactionVM.Rounding) != pTransactionVM.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (pTransactionVM.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, pTransactionVM.Amount) });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == pTransactionVM.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if (terminalUser.OpenDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });


            if (await _context.Transactions
                           .Include(x => x.TypeTransaction)
                           .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                       x.DateTransaction.Date == DateTime.UtcNow.ToLocalTime().Date &&
                                       (x.TypeTransaction.Id == 5 || x.TypeTransaction.Id == 7))
                           .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });


            if (await _context.Transactions
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.UtcNow.ToLocalTime().Date.ToShortDateString() &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });

            if (await _context.Debts.Include(dd => dd.DebtDetails)
                                          .Where(gs => _context.Statuses
                                          .Any(s => s.GroupStatusId == 4 && s.CodeName == gs.Status) && gs.AgreementId == AgreementId)
                                          .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Improcedente anticipo. La cuenta tiene adeudo") });

            if(pTransactionVM.Type == "PAY04")
            {
                var _serviceParam = await _context.ServiceParams
                                              .Where(x => x.ServiceId == 17 && x.IsActive == true)
                                              .FirstOrDefaultAsync();

                _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
            }
            if(pTransactionVM.Type == "PAY06")
            {
                var _serviceParam = await _context.ServiceParams
                                              .Where(x => x.ServiceId == 18 && x.IsActive == true)
                                              .FirstOrDefaultAsync();

                _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
            }
            

            
            #endregion


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pTransactionVM.Sign;
                    transaction.Amount = pTransactionVM.Amount;
                    transaction.Aplication = pTransactionVM.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pTransactionVM.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pTransactionVM.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = pTransactionVM.Cancellation;
                    transaction.Tax = pTransactionVM.Tax;
                    transaction.Rounding = pTransactionVM.Rounding;
                    transaction.AuthorizationOriginPayment = pTransactionVM.AuthorizationOriginPayment;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pTransactionVM.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(pTransactionVM.OriginPaymentId).ConfigureAwait(false);
                    transaction.Total = pTransactionVM.Total;
                    transaction.Account = pTransactionVM.Account;
                    transaction.AccountNumber = pTransactionVM.AccountNumber;
                    transaction.NumberBank = pTransactionVM.NumberBank;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    TransactionDetail transactionDetail = new TransactionDetail();
                    transactionDetail.CodeConcept = "ANT01";
                    transactionDetail.Amount = transaction.Amount;
                    transactionDetail.Description = "PAGO ANTICIPADO";
                    transactionDetail.Transaction = transaction;
                    _context.TransactionDetails.Add(transactionDetail);
                    await _context.SaveChangesAsync();

                    prepaid = new Prepaid();
                    prepaid.PrepaidDate = transaction.DateTransaction;
                    prepaid.Amount = transaction.Amount;
                    prepaid.Accredited = 0;
                    prepaid.Status = "ANT01";
                    prepaid.Type = pTransactionVM.Type;
                    prepaid.Percentage = pTransactionVM.Percentage;
                    prepaid.AgreementId = AgreementId;
                    _context.Prepaids.Add(prepaid);
                    await _context.SaveChangesAsync();

                    //Inserta pago
                    Payment payment = new Payment();
                    payment.PaymentDate = transaction.DateTransaction;
                    payment.BranchOffice = terminalUser.Terminal.BranchOffice.Name;
                    payment.Subtotal = transaction.Amount;
                    payment.PercentageTax = pTransactionVM.PercentageTax;
                    payment.Tax = transaction.Tax;
                    payment.Total = transaction.Amount + transaction.Tax + transaction.Rounding;
                    payment.AuthorizationOriginPayment = transaction.AuthorizationOriginPayment;
                    payment.AgreementId = prepaid.AgreementId;                    
                    payment.Status = "EP001";
                    payment.Type = pTransactionVM.Type;
                    payment.OriginPayment = transaction.OriginPayment;
                    payment.PayMethod = await _context.PayMethods.FindAsync(transaction.PayMethodId);
                    payment.TransactionFolio = transaction.Folio;
                    payment.Rounding = transaction.Rounding;
                    payment.ExternalOriginPayment = transaction.ExternalOriginPayment;
                    payment.Account = transaction.Account;
                    payment.AccountNumber = transaction.AccountNumber;
                    payment.NumberBank = transaction.NumberBank;
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    PaymentDetail paymentDetail = new PaymentDetail();
                    paymentDetail.CodeConcept = "ANT01";
                    paymentDetail.Amount = transaction.Amount;
                    paymentDetail.AccountNumber = _accountNumber;
                    paymentDetail.Description = "PAGO ANTICIPADO";
                    paymentDetail.DebtId = 0;
                    paymentDetail.PrepaidId = prepaid.Id;
                    paymentDetail.PaymentId = payment.Id;
                    paymentDetail.HaveTax = false;
                    paymentDetail.Tax = 0;
                    paymentDetail.Type = "TIP05";
                    paymentDetail.UnitMeasurement = _unitMeasurement;
                    _context.PaymentDetails.Add(paymentDetail);
                    await _context.SaveChangesAsync();

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pTransactionVM);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }


            return Ok(transaction.Id);
        }

        /// <summary>
        /// This will provide capability add new Transaction
        /// </summary>       
        /// <param name="TransactionId">Id Transaction
        /// <param name="pCancelPayment">Model PaymentConcepts
        /// </param>
        /// <returns>Transaction</returns>
        // POST: api/Transaction/Cancel
        [HttpPost("Prepaid/Cancel/{TransactionId}")]
        public async Task<IActionResult> PostTransactionPrepaidCancel([FromRoute] int TransactionId, [FromBody] CancelPaymentVM pCancelPayment)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            Prepaid prepaid;
            Payment payment = new Payment();
            PaymentDetail paymentDetail = new PaymentDetail();
            decimal _sumTransactionDetail = 0;
            decimal sumPayDetail = 0;
            decimal _saldo = 0;

            #region Validación
            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pCancelPayment.Transaction.TypeTransactionId != 4)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Acción no permitida" });

            if (!Validate(pCancelPayment.Transaction))
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Información incompleta" });

            if (pCancelPayment.Transaction.Sign)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });

            if (pCancelPayment.Transaction.Amount<=0)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cancelar incorrecto") });

            if (String.IsNullOrEmpty(pCancelPayment.Transaction.Cancellation))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

            foreach (var item in pCancelPayment.Transaction.transactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((pCancelPayment.Transaction.Amount + pCancelPayment.Transaction.Tax + pCancelPayment.Transaction.Rounding) != pCancelPayment.Transaction.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (pCancelPayment.Transaction.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, pCancelPayment.Transaction.Amount) });

            if (pCancelPayment.Payment.PaymentDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No es posible cancelar pagos de otros días") });

            foreach (var item in pCancelPayment.Payment.PaymentDetails)
            {
                sumPayDetail += item.Amount;
            }
            if (pCancelPayment.Transaction.Amount != sumPayDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de detalle de pago no coinciden") });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == pCancelPayment.Transaction.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            if (!terminalUser.InOperation)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando" });

            if (terminalUser.OpenDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });

            if (await _context.Transactions
                           .Include(x => x.TypeTransaction)
                           .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                       x.DateTransaction.Date == DateTime.UtcNow.ToLocalTime().Date &&
                                       (x.TypeTransaction.Id == 5 || x.TypeTransaction.Id == 7))
                           .FirstOrDefaultAsync() != null)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "El estado de la terminal no permite la transacción" });


            if (await _context.Transactions
                              .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                          x.DateTransaction.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString() &&
                                          x.TypeTransaction.Id == 1)
                              .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Debe aperturar una terminar para realizar una transacción" });

            var movimientosCaja = await _context.Transactions
                                               .Include(x => x.TypeTransaction)
                                               .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                          x.PayMethodId == pCancelPayment.Transaction.PayMethodId &&
                                                         (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                               .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x => {
                _saldo += x.Sign ? x.Total : x.Total * -1;
            });


            if (pCancelPayment.Transaction.Amount > _saldo)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No hay fondos suficientes en caja para la devolución") });

            //Cancelación
            var cancelacion = await _context.Transactions.Where(x => x.Folio == pCancelPayment.Transaction.Cancellation).FirstAsync();
            if (cancelacion == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

            var cancelacionPrevia = await _context.Transactions
                                                 .Include(x => x.TransactionFolios)
                                                 .Where(x => x.CancellationFolio == pCancelPayment.Transaction.Cancellation).FirstOrDefaultAsync();
            if (cancelacionPrevia != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago ha sido cancelado previamente. Folio->{0}", cancelacionPrevia.TransactionFolios.FirstOrDefault().Folio) });

            if (cancelacion.Amount != pCancelPayment.Transaction.Amount)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de cancelación no coincide con el pago") });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pCancelPayment.Transaction.Sign;
                    transaction.Amount = pCancelPayment.Transaction.Amount;
                    transaction.Aplication = pCancelPayment.Transaction.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pCancelPayment.Transaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pCancelPayment.Transaction.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = pCancelPayment.Transaction.Cancellation;
                    transaction.Tax = pCancelPayment.Transaction.Tax;
                    transaction.Rounding = pCancelPayment.Transaction.Rounding;
                    transaction.AuthorizationOriginPayment = pCancelPayment.Transaction.AuthorizationOriginPayment;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(pCancelPayment.Transaction.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(pCancelPayment.Transaction.OriginPaymentId).ConfigureAwait(false);
                    transaction.Total = pCancelPayment.Transaction.Total;
                    transaction.Account = pCancelPayment.Transaction.Account;
                    transaction.AccountNumber = pCancelPayment.Transaction.AccountNumber;
                    transaction.NumberBank = pCancelPayment.Transaction.NumberBank;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    TransactionDetail transactionDetail = new TransactionDetail();
                    transactionDetail.CodeConcept = "ANT01";
                    transactionDetail.Amount = transaction.Amount;
                    transactionDetail.Description = "CANCELACION PAGO ANTICIPADO";
                    transactionDetail.Transaction = transaction;
                    _context.TransactionDetails.Add(transactionDetail);
                    await _context.SaveChangesAsync();

                    //se modifica estado de pago
                    payment = await _context.Payments
                                            .Include(x => x.PaymentDetails)
                                            .Where(x => x.TransactionFolio == transaction.CancellationFolio).FirstOrDefaultAsync();

                    if (payment.PaymentDetails.Count > 0)
                    {
                        foreach (var item in payment.PaymentDetails)
                        {
                            paymentDetail = item;
                        }
                    }
                    else
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No se cuenta con detalle de pago para poder revertir") });

                    prepaid = await _context.Prepaids.Where(x => x.Id == paymentDetail.PrepaidId).FirstAsync();

                    if (prepaid == null)
                        return NotFound();
                    else
                    {
                        var prepaidDetail = await _context.PrepaidDetails.Where(x => x.PrepaidId == prepaid.Id).FirstOrDefaultAsync();

                        if (prepaid.Status != "ANT01" || prepaidDetail != null)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago no se puede cancelar porque el ya ha sido devengado el anticipo") });
                    }

                    payment.Status = "EP002";
                    _context.Entry(payment).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    prepaid.Status = "ANT04";
                    _context.Entry(prepaid).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pCancelPayment);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
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
                     

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pTransaction.TerminalUserId == 0 || pTransaction.TypeTransactionId == 0)
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

            if (terminalUser.OpenDate.Date != DateTime.UtcNow.ToLocalTime().Date)
                return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal no se encuentra operando el día de hoy" });

            switch (pTransaction.TypeTransactionId)
            {
                case 1://apertura
                    if (await _context.Transactions
                                     .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                 x.TypeTransactionId == 1)
                                     .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha aperturado" });
                  
                        pTransaction.Amount = 0;
                    break;
                case 2://Fondo
                    if (await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 1)
                                   .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    var fondo = await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 2)
                                   .FirstOrDefaultAsync();

                    if (fondo != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha ingresado un fondo de caja" });
                   
                    if (pTransaction.Amount == 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar un fondo de caja") });

                    if (pTransaction.PayMethodId == 0)
                            return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Falta método de pago" });

                        if (terminalUser.Terminal.CashBox > 0)
                        {
                            if (pTransaction.Amount > terminalUser.Terminal.CashBox)
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de fondo de caja debe ser menor a: ${0}", terminalUser.Terminal.CashBox) });
                        }
                   
                    break;
                case 3://Cobro
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Acción no permitida" });
                case 4://Cancelación
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Acción no permitida" });
                case 5://Cierre
                    if (await _context.Transactions
                                  .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                              x.TypeTransactionId == 1)
                                  .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    if (await _context.Transactions
                                    .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                x.TypeTransactionId == 7)
                                    .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal debe ser liquidada previamente") });

                    if (await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 5)
                                   .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha sido cerrada" });
                  
                        pTransaction.Amount = 0;
                        terminalUser.InOperation = false;
                        _context.Entry(terminalUser).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    break;
                case 6://Retiro
                    if (await _context.Transactions
                                  .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                              x.TypeTransactionId == 1)
                                  .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    if (await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 7)
                                   .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal ya ha sido liquidada") });

                    if (await _context.Transactions
                                 .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                             x.TypeTransactionId == 5)
                                 .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha sido cerrada" });


                    if (pTransaction.Sign)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                    if (pTransaction.Amount <= 0)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a retirar incorrecto") });
                    if (pTransaction.PayMethodId == 0)
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Método de pago incorrecto" });

                    var movimientos = await _context.Transactions
                                                    .Include(x => x.TypeTransaction)
                                                    .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                                x.PayMethodId == pTransaction.PayMethodId &&
                                                                (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                                    .OrderBy(x => x.Id).ToListAsync();

                    if (movimientos == null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de retiro incorrecto" });

                    decimal _saldo = 0;

                    movimientos.ForEach(x => {
                        _saldo += x.Sign ? x.Total : x.Total * -1;
                    });

                    if (pTransaction.Amount > _saldo)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No hay fondos suficientes en caja para el retiro") });
                    break;
                case 7://Liquidada
                    if (await _context.Transactions
                                  .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                              x.TypeTransactionId == 1)
                                  .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    if (await _context.Transactions
                                 .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                             x.TypeTransactionId == 7)
                                 .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal ya ha sido liquidada") });

                    if (pTransaction.Sign)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de liquidación incorrecta") });
                    if (pTransaction.Amount < 0)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a liquidar incorrecto") });
                    if (pTransaction.PayMethodId == 0)
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Especificar método de pago" });

                    var liquidar = await _context.Transactions
                                 .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                             x.TypeTransactionId == 2)
                                 .FirstOrDefaultAsync();

                    if (liquidar != null)
                    if (pTransaction.Amount != liquidar.Amount)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto de liquidación incorrecto") });
                    break;
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pTransaction.Sign;
                    transaction.Amount = pTransaction.Amount;
                    transaction.Aplication = pTransaction.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pTransaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pTransaction.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = String.Empty;
                    transaction.Tax = 0;
                    transaction.Rounding = 0;
                    transaction.AuthorizationOriginPayment = String.Empty;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(1).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(1).ConfigureAwait(false);
                    transaction.Total = pTransaction.Amount;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    if (pTransaction.TypeTransactionId == 2 || pTransaction.TypeTransactionId == 6 || pTransaction.TypeTransactionId == 7)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pTransaction.TypeTransactionId.ToString();
                        transactionDetail.Amount = transaction.Amount;
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
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pTransaction);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }
            return Ok(transaction.Id);
        }


        /// <summary>
        /// Create a cash box operation
        /// </summary>       
        /// /// <param name="teminalUserId">Model TransactionVM
        /// <param name="pTransaction">Model TransactionVM
        /// </param>
        /// <returns>New Transaction added</returns>
        // POST: api/Transaction
        [Authorize(Policy = "RequireSupervisorRole")]
        [HttpPost("Super/{teminalUserId}")]
        public async Task<IActionResult> PostTransactionCashBoxSuper([FromRoute] int teminalUserId, [FromBody] TransactionCashBoxVM pTransaction)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pTransaction.TerminalUserId == 0 || pTransaction.TypeTransactionId == 0)
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

            
            switch (pTransaction.TypeTransactionId)
            {
                case 1://apertura
                    if (await _context.Transactions
                                     .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                 x.TypeTransactionId == 1)
                                     .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha aperturado" });

                    pTransaction.Amount = 0;
                    break;
                case 2://Fondo
                    if (await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 1)
                                   .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    var fondo = await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 2)
                                   .FirstOrDefaultAsync();

                    if (fondo != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha ingresado un fondo de caja" });

                    if (pTransaction.Amount == 0)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar un fondo de caja") });

                    if (pTransaction.PayMethodId == 0)
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Falta método de pago" });

                    if (terminalUser.Terminal.CashBox > 0)
                    {
                        if (pTransaction.Amount > terminalUser.Terminal.CashBox)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El monto de fondo de caja debe ser menor a: ${0}", terminalUser.Terminal.CashBox) });
                    }

                    break;
                case 3://Cobro
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Acción no permitida" });
                case 4://Cancelación
                    return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Acción no permitida" });
                case 5://Cierre
                    if (await _context.Transactions
                                  .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                              x.TypeTransactionId == 1)
                                  .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    if (await _context.Transactions
                                    .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                x.TypeTransactionId == 7)
                                    .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal debe ser liquidada previamente") });

                    if (await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 5)
                                   .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha sido cerrada" });

                    pTransaction.Amount = 0;
                    terminalUser.InOperation = false;
                    _context.Entry(terminalUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    break;
                case 6://Retiro
                    if (await _context.Transactions
                                  .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                              x.TypeTransactionId == 1)
                                  .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    if (await _context.Transactions
                                   .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                               x.TypeTransactionId == 7)
                                   .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal ya ha sido liquidada") });

                    if (await _context.Transactions
                                 .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                             x.TypeTransactionId == 5)
                                 .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "La terminal ya ha sido cerrada" });


                    if (pTransaction.Sign)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de transacción incorrecta") });
                    if (pTransaction.Amount <= 0)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a retirar incorrecto") });
                    if (pTransaction.PayMethodId == 0)
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Método de pago incorrecto" });

                    var movimientos = await _context.Transactions
                                                    .Include(x => x.TypeTransaction)
                                                    .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                                x.PayMethodId == pTransaction.PayMethodId &&
                                                                (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                                    .OrderBy(x => x.Id).ToListAsync();

                    if (movimientos == null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de retiro incorrecto" });

                    decimal _saldo = 0;

                    movimientos.ForEach(x => {
                        _saldo += x.Sign ? x.Total : x.Total * -1;
                    });

                    if (pTransaction.Amount > _saldo)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No hay fondos suficientes en caja para el retiro") });
                    break;
                case 7://Liquidada
                    if (await _context.Transactions
                                  .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                              x.TypeTransactionId == 1)
                                  .FirstOrDefaultAsync() == null)
                        return StatusCode((int)TypeError.Code.NotAcceptable, new { Error = "Debe aperturar una terminar para realizar una transacción" });

                    if (await _context.Transactions
                                 .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                             x.TypeTransactionId == 7)
                                 .FirstOrDefaultAsync() != null)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("La terminal ya ha sido liquidada") });

                    if (pTransaction.Sign)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Naturaleza de liquidación incorrecta") });
                    if (pTransaction.Amount < 0)
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a liquidar incorrecto") });
                    if (pTransaction.PayMethodId == 0)
                        return StatusCode((int)TypeError.Code.BadRequest, new { Error = "Especificar método de pago" });

                    var liquidar = await _context.Transactions
                                 .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                             x.TypeTransactionId == 2)
                                 .FirstOrDefaultAsync();

                    if (liquidar != null)
                        if (pTransaction.Amount != liquidar.Amount)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto de liquidación incorrecto") });
                    break;
            }
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    transaction.DateTransaction = DateTime.UtcNow.ToLocalTime();
                    transaction.Sign = pTransaction.Sign;
                    transaction.Amount = pTransaction.Amount;
                    transaction.Aplication = pTransaction.Aplication;
                    transaction.TypeTransaction = await _context.TypeTransactions.FindAsync(pTransaction.TypeTransactionId).ConfigureAwait(false);
                    transaction.PayMethodId = pTransaction.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = String.Empty;
                    transaction.Tax = 0;
                    transaction.Rounding = 0;
                    transaction.AuthorizationOriginPayment = String.Empty;
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(1).ConfigureAwait(false);
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(1).ConfigureAwait(false);
                    transaction.Total = pTransaction.Amount;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    if (pTransaction.TypeTransactionId == 2 || pTransaction.TypeTransactionId == 6 || pTransaction.TypeTransactionId == 7)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pTransaction.TypeTransactionId.ToString();
                        transactionDetail.Amount = transaction.Amount;
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
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = "TransactionController";
                systemLog.Action = "PostTransaction";
                systemLog.Parameter = JsonConvert.SerializeObject(pTransaction);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }
            return Ok(transaction.Id);
        }

        private bool Validate(TransactionVM ptransaction)
        {
            if (ptransaction.PayMethodId == 0)
                return false;
            if (ptransaction.TerminalUserId == 0)
                return false;
            if (ptransaction.TypeTransactionId == 0)
                return false;
            if (ptransaction.ExternalOriginPaymentId == 0)
                return false;
            if (ptransaction.OriginPaymentId == 0)
                return false;
            if (ptransaction.Amount == 0)
                return false;

            return true;
        }
    }
}
