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
using System.Data;
using System.Data.SqlClient;
using System.Web.Http.Cors;
using Siscom.Agua.Api.Data;
using System.Net.Http;

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
            transactionPayment.Transaction = await _context.Transactions
                                                            .Include(x => x.OriginPayment)
                                                            .Include(x => x.ExternalOriginPayment)
                                                            //.Include(x => x.PayMethod)
                                                            .Include(x => x.TerminalUser)
                                                                 .ThenInclude(y => y.Terminal)
                                                                       .ThenInclude(z => z.BranchOffice)
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
                                                        .Include(P => P.TaxReceipts)
                                                        .Where(m => m.TransactionFolio == ((transactionPayment.Transaction.TypeTransaction.Id != 4) ? transactionPayment.Transaction.Folio : transactionPayment.Transaction.CancellationFolio))
                                                        .FirstOrDefaultAsync();

            //Determino si fue pago de adeudo o producto.
            if (transactionPayment.Payment.OrderSaleId != 0) //Order Sale
            {
                if (transactionPayment.Payment != null)
                {
                    transactionPayment.OrderSale = _context.OrderSales
                                                        .Include(os => os.OrderSaleDetails)
                                                        .Include(os => os.OrderSaleDiscounts)
                                                    .Where(os => os.Id == transactionPayment.Payment.OrderSaleId)
                                                    .FirstOrDefault();
                }
                transactionPayment.ClavesProdServ = transactionPayment.OrderSale.OrderSaleDetails.Select(o => new ClavesProductoServicioSAT() { CodeConcep = o.CodeConcept, Tipo = "TIP02", ClaveProdServ = _context.ProductParams.FirstOrDefault(x => x.ProductId == int.Parse(o.CodeConcept)).CodeConcept }).ToList();
            }
            else //Servicio
            {
                if (transactionPayment.Payment != null && transactionPayment.Payment.Type != "PAY04")
                {
                    transactionPayment.Payment.PaymentDetails.ToList().ForEach(x =>
                    {
                        x.Debt = _context.Debts.Include(dd => dd.DebtDiscounts).Include(dd => dd.DebtDetails).Where(d => d.Id == x.DebtId).FirstOrDefault();
                        x.Prepaid = _context.Prepaids.Find(x.PrepaidId);
                    });
                }

                List<ClavesProductoServicioSAT> tmpClaves = new List<ClavesProductoServicioSAT>();
                foreach (var item in transactionPayment.Payment.PaymentDetails)
                {
                    if (transactionPayment.Payment.Type == "PAY04")
                    {
                        var tmpConceptos = new ClavesProductoServicioSAT() { CodeConcep = item.CodeConcept, Tipo = item.Type, ClaveProdServ = _context.ServiceParams.FirstOrDefault(x => x.ServiceId == int.Parse(item.CodeConcept)).CodeConcept };
                        tmpClaves.Add(tmpConceptos);
                    }
                    else
                    {
                        if (item.Type != "TIP02" && item.Type != "TIP03")
                        {
                            var tmpConceptos = item.Debt.DebtDetails.Select(d => new ClavesProductoServicioSAT() { CodeConcep = d.CodeConcept, Tipo = item.Type, ClaveProdServ = _context.ServiceParams.FirstOrDefault(x => x.ServiceId == int.Parse(d.CodeConcept)).CodeConcept });
                            tmpClaves.AddRange(tmpConceptos.ToList());
                        }
                        else    //Producto con cuenta.
                        {
                            var tmpConceptos = item.Debt.DebtDetails.Select(d => new ClavesProductoServicioSAT() { CodeConcep = d.CodeConcept, Tipo = item.Type, ClaveProdServ = _context.ProductParams.FirstOrDefault(x => x.ProductId == int.Parse(d.CodeConcept)).CodeConcept });
                            tmpClaves.AddRange(tmpConceptos.ToList());
                        }
                    }
                }
                transactionPayment.ClavesProdServ = tmpClaves.GroupBy(c => new { c.ClaveProdServ, c.Tipo, c.CodeConcep }).Select(x => x.First()).ToList();
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

        // GET: api/TerminalUser
        [HttpGet("{date}/{UserId}")]
        public async Task<IActionResult> FindTransactionsFromDate([FromRoute] string date, string UserId)
        {

            DateTime fromDate;
            DateTime.TryParse(date, out fromDate);

            var transaction = await _context.Transactions
                                     .Include(x => x.TypeTransaction)
                                     .Include(x => x.TransactionFolios)
                                     .Where(x => x.TerminalUser.UserId == UserId && x.DateTransaction.Date == fromDate)
                                     .OrderBy(x => x.Id).ToListAsync();
            if (transaction == null)
            {
                return NotFound();
            }

            transaction.ToList().ForEach(x =>
            {
                x.PayMethod = _context.PayMethods.Find(x.PayMethodId);
            });

            return Ok(transaction);
        }

        // GET: api/TerminalUser
        [HttpGet("Find/{date}/{terminalUserId}")]
        public async Task<IActionResult> FindTransactionsDate([FromRoute] string date, int terminalUserId)
        {
            var transactionn = await (from t
                                        in _context.Transactions
                                                    .Include(x => x.TypeTransaction)
                                                    .Include(x => x.TransactionFolios)
                                      join p in _context.Payments
                                                        .Include(x => x.TaxReceipts)
                                             on t.Folio equals p.TransactionFolio
                                      into tran
                                      from defaultVal
                                          in tran.DefaultIfEmpty()
                                      orderby t.Id
                                      where t.TerminalUserId == terminalUserId
                                      select new
                                      {
                                          t,
                                          p = defaultVal
                                      }).ToListAsync();

            var transaction = transactionn.Select(x => new Siscom.Agua.DAL.Models.Transaction
            {
                Account = x.t.Account,
                AccountNumber = x.t.AccountNumber,
                Amount = x.t.Amount,
                Aplication = x.t.Aplication,
                AuthorizationOriginPayment = x.t.AuthorizationOriginPayment,
                CancelAuthorizationId = x.t.CancelAuthorizationId,
                CancellationFolio = x.t.CancellationFolio,
                DateTransaction = x.t.DateTransaction,
                ExternalOriginPayment = x.t.ExternalOriginPayment,
                ExternalOriginPaymentId = x.t.ExternalOriginPaymentId,
                Folio = x.t.Folio,
                Id = x.t.Id,
                NumberBank = x.t.NumberBank,
                OriginPayment = x.t.OriginPayment,
                OriginPaymentId = x.t.OriginPaymentId,
                Payment = x.p,
                PayMethod = x.t.PayMethod,
                PayMethodId = x.t.PayMethodId,
                PayMethodNumber = x.t.PayMethodNumber,
                Rounding = x.t.Rounding,
                Sign = x.t.Sign,
                Tax = x.t.Tax,
                TerminalUser = x.t.TerminalUser,
                TerminalUserId = x.t.TerminalUserId,
                Total = x.t.Total,
                TransactionDetails = x.t.TransactionDetails,
                TransactionFolios = x.t.TransactionFolios,
                TypeTransaction = x.t.TypeTransaction,
                TypeTransactionId = x.t.TypeTransactionId,
            }).ToList();

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
        //Obtiene las transacciones de un usuario para un dia especifico.
        [HttpGet("FromUserInDay/{date}/{idUser}")]
        public async Task<IActionResult> FindTransactionsFromUserInDay([FromRoute] string date, string idUser)
        {
            var idTerminal = _context.TerminalUsers.Where(t => t.UserId == idUser && t.OpenDate.ToString("yyyy-MM-dd") == date).Select(t => t.Id).ToList();

            List<DAL.Models.Transaction> transaction = new List<DAL.Models.Transaction>();
            foreach (var item in idTerminal)
            {
                //Obtengo los correspondientes a la terminal del usuario y que son cobros (TypeTransactionId == 3)
                transaction.AddRange(await _context.Transactions
                                         .Include(x => x.TypeTransaction)
                                         .Include(x => x.TransactionFolios)
                                         .Where(x => x.TerminalUser.Id == item && (x.TypeTransactionId == 3 || x.TypeTransactionId == 4))
                                         .OrderBy(x => x.Id).ToListAsync());
            }

            if (transaction == null)
            {
                return NotFound();
            }

            //Obtengo los pagos correspondientes a las transacciones, que esten activos.
            var lstFolios = transaction.Where(x => x.TypeTransactionId == 3).Select(t => t.Folio).ToList();
            var pays = await _context.Payments.Where(p => lstFolios.Contains(p.TransactionFolio) && p.Status != "EP002").Select(pp => pp.TransactionFolio).ToListAsync();
            var TransactionsFiltrados = transaction.Where(t => pays.Contains(t.Folio)).ToList();

            //Obtengo los pagos cancelados correspondientes a las transacciones, que esten activos.
            var lstFoliosC = transaction.Where(x => x.TypeTransactionId == 4).Select(t => t.CancellationFolio).ToList();
            var paysC = await _context.Payments.Where(p => lstFoliosC.Contains(p.TransactionFolio) && p.Status == "EP002").Select(pp => pp.TransactionFolio).ToListAsync();
            TransactionsFiltrados.AddRange(transaction.Where(t => paysC.Contains(t.CancellationFolio)).ToList());

            List<TransactionMovimientosCaja> lstMovs = TransactionsFiltrados
                .Select(t => new TransactionMovimientosCaja()
                {
                    IdTransaction = t.Id,
                    FolioTransaccion = t.TypeTransactionId == 3 ? t.Folio : t.CancellationFolio,
                    Cuenta = t.TypeTransactionId == 3 ? t.Account : transaction.Where(x => x.Folio == t.CancellationFolio).FirstOrDefault().Account ,
                    Operacion = t.TypeTransaction.Name,
                    FolioImpresion = t.TypeTransactionId == 3 ? (t.TransactionFolios.Count > 0 ? t.TransactionFolios.FirstOrDefault().Folio : "") : "---",
                    Hora = t.DateTransaction.ToString("hh:mm tt"),
                    Total = t.Total,
                    Signo = t.Sign
                })
                .ToList();

            lstMovs.ToList().ForEach(x =>
            {
                if (x.Operacion == "Cobro" || x.Operacion.Contains("Cancelaci"))
                {
                    x.HaveInvoice = _context.Payments.Where(p => p.TransactionFolio == x.FolioTransaccion).FirstOrDefault().HaveTaxReceipt;
                    x.IdPayment = _context.Payments.Where(p => p.TransactionFolio == x.FolioTransaccion).FirstOrDefault().Id;
                    if (_context.Payments.Where(p => p.TransactionFolio == x.FolioTransaccion).FirstOrDefault().AgreementId != 0)
                    {
                        var idAgreement = _context.Payments.Where(p => p.TransactionFolio == x.FolioTransaccion).FirstOrDefault().AgreementId;
                        var Cliente = _context.Clients.Where(c => c.AgreementId == idAgreement && c.TypeUser == "CLI01").FirstOrDefault();
                        x.Cliente = Cliente != null ? Cliente.Name + " " + Cliente.LastName + " " + Cliente.SecondLastName : "Contribuyente";
                    }
                    else
                    {
                        var idOS = _context.Payments.Where(p => p.TransactionFolio == x.FolioTransaccion).FirstOrDefault().OrderSaleId;
                        var OS = _context.OrderSales.Where(o => o.Id == idOS).FirstOrDefault();
                        var taxUser = _context.TaxUsers.Where(t => t.Id == OS.TaxUserId && t.IsActive == true).FirstOrDefault();
                        x.Cliente = taxUser != null ? taxUser.Name : "Contribuyente";
                    }
                }
                else
                    x.HaveInvoice = false;
            });

            return Ok(lstMovs);
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

            if (entities.Count == 0)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = "No hay corte" });

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

            if (paymentsFacturadosFinal.Count > 0)
                transactionPayment.lstPayment = lstPaymentRelacionadosATransacciones.Where(pp => !paymentsFacturadosFinal.Contains(pp.Id)).ToList();
            else
                transactionPayment.lstPayment = lstPaymentRelacionadosATransacciones;

            //Obtengo los id´s de folios en Payments para filtral los transactions.
            var lstIdsFoliosPayments = transactionPayment.lstPayment.Select(y => y.TransactionFolio).ToList();
            transactionPayment.lstTransaction = lstTransations.Where(yy => lstIdsFoliosPayments.Contains(yy.Folio)).ToList();

            if (transactionPayment.lstPayment == null)
            {
                return NotFound();
            }

            return Ok(transactionPayment);
        }

        [HttpGet("TransactionPaymentWithTaxGrouped/{date}/{BranchOfficeId}/{TypeTransactionId}")]
        public async Task<IActionResult> GetTransactionPaymentWithTaxGrouped([FromRoute] string date, int BranchOfficeId, int TypeTransactionId)
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
                                            .Include(x => x.TransactionFolios)
                                            .Where(x => x.TypeTransactionId == TypeTransactionId && x.DateTransaction >= tmpFechaStart && x.DateTransaction <= tmpFechaEnd).ToListAsync();
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
                                                        .Include(p => p.PaymentDetails)
                                                        .Include(p => p.TaxReceipts)
                                                        .Where(p => lstIds.Contains(p.TransactionFolio) && p.BranchOffice == BranchOffice.Name)
                                                        .ToListAsync();

            //Obtengo solo los taxReceipts para buscar los agrupados.
            var lstIdsBranch = lstPaymentRelacionadosATransacciones.Select(x => x.Id).ToList();

            //De los TaxReceipt verifico cuales son agrupados.
            var lstTaxReceiptAgrupados = _context.TaxReceipts
                                                .Where(x => lstIdsBranch.Contains(x.PaymentId) && (x.Status == "ET001" || x.Status == "ET002"))
                                                .GroupBy(y => y.FielXML)
                                                .Select(y => new { fieldXML = y.Key, total = y.Count(), grupo = y.ToList() }).ToList();

            //Agrego los payment con mas de una factura.
            List<Payment> lstPaymentFinal = new List<Payment>();
            foreach (var item in lstTaxReceiptAgrupados)
            {
                if (item.total > 1)
                {
                    foreach (var elem in item.grupo)
                    {
                        Payment pay = lstPaymentRelacionadosATransacciones.Where(x => x.Id == elem.PaymentId).FirstOrDefault();
                        lstPaymentFinal.Add(pay);
                    }
                }
            }

            transactionPayment.lstPayment = lstPaymentFinal;

            //Obtengo los id´s de folios en Payments para filtral los transactions.
            var lstIdsFoliosPayments = lstPaymentFinal.Select(y => y.TransactionFolio).ToList();
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
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Naturaleza de transacción incorrecta" });

            if (await _context.Agreements
                             .Where(x => x.Account == pPaymentConcepts.Transaction.Account)
                             .FirstOrDefaultAsync() == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El número de cuenta no es correcto" });

            foreach (var item in pPaymentConcepts.Transaction.transactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((pPaymentConcepts.Transaction.Amount + pPaymentConcepts.Transaction.Tax + pPaymentConcepts.Transaction.Rounding) != pPaymentConcepts.Transaction.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });


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
                            {
                                var validIva = Convert.ToDecimal((Math.Round(y.OnPayment * Convert.ToDecimal(.16), 2)));
                                if (validIva != y.Tax)
                                    _validation = false;
                            }

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

            if (pPaymentConcepts.Transaction.Tax != _sumTaxDebtDetail)
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
                    payment.HaveTaxReceipt = transaction.Tax > 0 ? true : false;
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
                                                                          .Where(x => x.ServiceId == Convert.ToInt32(!string.IsNullOrWhiteSpace(detail.CodeConcept) ? detail.CodeConcept : "0"))
                                                                          .FirstOrDefaultAsync();

                                        _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                                        _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
                                    }
                                    else
                                    {
                                        var _productParam = await _context.ProductParams
                                                                          .Where(x => x.ProductId == Convert.ToInt32(!string.IsNullOrWhiteSpace(detail.CodeConcept) ? detail.CodeConcept : "0"))
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
            int count = 0;
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

            if (await _context.OrderSales
                              .Where(x => x.Folio == pPaymentOrders.Transaction.Account)
                              .FirstOrDefaultAsync() == null)
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

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    //PAGOS                           
                    payment.PaymentDate = transaction.DateTransaction;
                    payment.BranchOffice = terminalUser.Terminal.BranchOffice.Name;
                    payment.Subtotal = transaction.Amount;
                    payment.PercentageTax = pPaymentOrders.Transaction.PercentageTax;
                    payment.Tax = transaction.Tax;
                    payment.Rounding = Math.Truncate(transaction.Rounding * 100) / 100;
                    payment.Total = transaction.Total;
                    payment.AuthorizationOriginPayment = transaction.AuthorizationOriginPayment;
                    payment.TransactionFolio = transaction.Folio;
                    payment.AgreementId = pPaymentOrders.Transaction.AgreementId;
                    payment.Status = "EP001";
                    payment.Type = pPaymentOrders.Transaction.Type;
                    payment.HaveTaxReceipt = transaction.Tax > 0 ? true : false;
                    payment.OriginPayment = transaction.OriginPayment;
                    payment.ExternalOriginPayment = transaction.ExternalOriginPayment;
                    payment.PayMethod = await _context.PayMethods.FindAsync(transaction.PayMethodId);
                    //NumberOfCheque
                    payment.AccountNumber = transaction.AccountNumber;
                    //NumberCardBank
                    payment.NumberBank = transaction.NumberBank;
                    //Account
                    payment.Account = transaction.Account;
                    payment.OrderSaleId = pPaymentOrders.Transaction.OrderSaleId;

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    //Movimientos a deuda
                    foreach (var order in pPaymentOrders.OrderSale)
                    {
                        //Recibo a pagar
                        var orderFind = await _context.OrderSales
                                                        .Include(x => x.OrderSaleDetails)
                                                        .Where(x => x.Id == order.Id).FirstOrDefaultAsync();

                        if (!String.IsNullOrEmpty(order.Status))
                        {

                            if (await _context.Statuses
                                                      .Where(x => x.GroupStatusId == 10 &&
                                                                  x.CodeName == orderFind.Status).FirstAsync() != null)
                            {
                                orderFind.Status = order.Status;
                                orderFind.OnAccount = order.OnAccount;
                                orderFind.OrderSaleDetails.ToList().ForEach(x =>
                                {
                                    x.OnAccount = order.OrderSaleDetails.Where(y => y.OrderSaleId == x.OrderSaleId).ToArray()[count].OnAccount;
                                    count++;
                                });
                                count = 0;
                                _context.Entry(orderFind).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                                //Conceptos
                                foreach (var detail in order.OrderSaleDetails)
                                {
                                    var conceptos = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == order.Id &&
                                                                                          x.Id == detail.Id).FirstOrDefaultAsync();

                                    if (conceptos.OnAccount != detail.OnAccount)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a pagar del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                    if (conceptos.OnAccount > conceptos.Amount)
                                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: conceptos.NameConcept) });

                                    conceptos.OnAccount = detail.OnAccount;
                                    _context.Entry(conceptos).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();

                                    string _accountNumber = String.Empty;
                                    string _unitMeasurement = String.Empty;


                                    if (orderFind.Type == "OA001" || orderFind.Type == "OM001")
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
                                    paymentDetail.Amount = detail.OnAccount;
                                    paymentDetail.Description = detail.NameConcept;
                                    paymentDetail.DebtId = 0;
                                    paymentDetail.PrepaidId = 0;
                                    paymentDetail.OrderSaleId = order.Id;
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


        [HttpPost("ResumeTransactions/{date}")]
        public async Task<IActionResult> ResumeTansactions([FromRoute] string date, [FromBody] string idUsers)
        {
            int tmpAño = int.Parse(date.Split("-")[0]);
            int tmpMes = int.Parse(date.Split("-")[1]);
            int tmpDia = int.Parse(date.Split("-")[2]);
            DateTime tmpFechaStart = new DateTime(tmpAño, tmpMes, tmpDia, 0, 0, 0);
            DateTime tmpFechaEnd = new DateTime(tmpAño, tmpMes, tmpDia, 23, 59, 59);

            string[] lstIds = idUsers.Split(',');

            try
            {
                List<DAL.Models.Transaction> res = new List<DAL.Models.Transaction>();
                res = await _context.Transactions
                    .Include(x => x.TypeTransaction)
                    .Include(x => x.TerminalUser)
                    .Include(x => x.TransactionDetails)
                    .Where(x => x.DateTransaction >= tmpFechaStart && x.DateTransaction < tmpFechaEnd && lstIds.Contains(x.TerminalUser.UserId))
                    .ToListAsync();

                foreach (var item in res)
                {
                    item.PayMethod = _context.PayMethods.Where(p => p.Id == item.PayMethodId).FirstOrDefault();
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                var tmp = ex.Message;
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la consulta." });
            }
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
            bool removeItem = false;

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
                                                          (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                               .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x =>
            {
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
                    _context.Transactions.Add(transaction);           //<-----------------------------------------------------------
                    await _context.SaveChangesAsync();                //<-----------------------------------------------------------

                    //se modifica estado de pago
                    payment = await _context.Payments.FindAsync(pCancelPayment.Payment.Id);
                    payment.Status = "EP002";
                    _context.Entry(payment).State = EntityState.Modified;     //<-----------------------------------------------------------
                    await _context.SaveChangesAsync();                        //<-----------------------------------------------------------

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
                                if (statusDebt.Count >= 1)
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

                            _context.Entry(debt).State = EntityState.Modified;        //<-----------------------------------------------------------
                            await _context.SaveChangesAsync();                        //<-----------------------------------------------------------

                            if (!removeItem)
                            {
                                _context.Entry(await _context.DebtStatuses.FindAsync(statusDebt.First().Id)).State = EntityState.Deleted;
                                await _context.SaveChangesAsync();
                                removeItem = true;
                            }

                            //DebtStatus debtStatus = new DebtStatus()
                            //{
                            //    id_status = debt.Status,
                            //    DebtStatusDate = transaction.DateTransaction,
                            //    User = terminalUser.User.Name + ' ' + terminalUser.User.LastName,
                            //    DebtId = debt.Id
                            //};
                            //_context.DebtStatuses.Add(debtStatus);                    //<-----------------------------------------------------------
                            //await _context.SaveChangesAsync();                        //<-----------------------------------------------------------

                            removeItem = false;
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
                        _context.TransactionDetails.Add(transactionDetail);       //<-----------------------------------------------------------
                        await _context.SaveChangesAsync();                        //<----------------------------------------------------------- 

                        DebtDetail debtDetail = new DebtDetail();
                        debtDetail = await _context.DebtDetails.Where(x => x.DebtId == pay.DebtId &&
                                                                           x.CodeConcept == pay.CodeConcept).FirstAsync();

                        if (debtDetail.OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        debtDetail.OnAccount -= pay.Amount;
                        await _context.SaveChangesAsync();                        //<-----------------------------------------------------------
                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
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

            if (pTransactionVM.Type == "PAY04")
            {
                var _serviceParam = await _context.ServiceParams
                                              .Where(x => x.ServiceId == 19 && x.IsActive == true)
                                              .FirstOrDefaultAsync();

                _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
            }
            if (pTransactionVM.Type == "PAY06")
            {
                var _serviceParam = await _context.ServiceParams
                                              .Where(x => x.ServiceId == 18 && x.IsActive == true)
                                              .FirstOrDefaultAsync();

                _accountNumber = _serviceParam != null ? _serviceParam.CodeConcept : String.Empty;
                _unitMeasurement = _serviceParam != null ? _serviceParam.UnitMeasurement : String.Empty;
            }

            var _codeconcept = _context.Services.Where(x => x.Name == "ANTICIPO").FirstOrDefault();


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
                    transactionDetail.CodeConcept = _codeconcept.Id.ToString();
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
                    paymentDetail.CodeConcept = _codeconcept.Id.ToString();
                    paymentDetail.Amount = transaction.Amount;
                    paymentDetail.AccountNumber = _accountNumber;
                    //paymentDetail.Description = "PAGO ANTICIPADO";
                    paymentDetail.Description = transaction.TransactionDetails.FirstOrDefault().Description;
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

            movimientosCaja.ForEach(x =>
            {
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

            var _codeconcept = _context.Services.Where(x => x.Name == "ANTICIPO").FirstOrDefault();

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
                    transactionDetail.CodeConcept = _codeconcept.Id.ToString();
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

        [HttpPost("Orders/Cancel/{TransactionId}")]
        public async Task<IActionResult> PostTransactionOrderCancel([FromRoute] int TransactionId, [FromBody] CancelPaymentVM pCancelPayment)
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

            movimientosCaja.ForEach(x =>
            {
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

            //var _codeconcept = _context.Services.Where(x => x.Name == "ANTICIPO").FirstOrDefault();

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

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    var orderList = pCancelPayment.Payment.PaymentDetails.Select(x => x.OrderSaleId).Distinct();

                    foreach (var item in orderList)
                    {
                        OrderSale order = await _context.OrderSales.FindAsync(item);
                        order.OnAccount = (order.Amount - order.OnAccount);
                        order.Status = "EOS01";
                        _context.Entry(order).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    int position = 0;

                    foreach (var pay in pCancelPayment.Payment.PaymentDetails)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pay.CodeConcept;
                        transactionDetail.Amount = transaction.Amount;
                        transactionDetail.Description = pay.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();

                        List<OrderSaleDetail> saleDetails = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == pay.OrderSaleId).ToListAsync();

                        if (saleDetails[position].OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        saleDetails[position].OnAccount -= pay.Amount;
                        saleDetails[position].Tax = 0;
                        await _context.SaveChangesAsync();
                        position++;
                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = JsonConvert.SerializeObject(pCancelPayment);
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok();
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

                    movimientos.ForEach(x =>
                    {
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

                    movimientos.ForEach(x =>
                    {
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

        [HttpPost("SuperService/Cancel/{TransactionId}/{CancelSAC?}")]
        public async Task<IActionResult> TransactionServiceCancel([FromRoute] int TransactionId, [FromQuery] bool CancelSAC = false)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            Prepaid prepaid;
            decimal _sumTransactionDetail = 0;
            decimal sumPayDetail = 0;
            decimal _saldo = 0;

            #region Validación
            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transactionData = await _context.Transactions
                                        .Include(x => x.TransactionFolios)
                                        .Include(t => t.TransactionDetails)
                                        .Where(x => x.Id == TransactionId).SingleOrDefaultAsync();

            var paymentData = await _context.Payments
                                            .Include(x => x.PaymentDetails)
                                            .Include(t => t.TaxReceipts)//then include cancelaciones
                                            .Where(x => x.TransactionFolio == transactionData.Folio)
                                            .SingleOrDefaultAsync();

            if (transactionData.Amount <= 0)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cancelar incorrecto") });

            if (String.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

            foreach (var item in transactionData.TransactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((transactionData.Amount + transactionData.Tax + transactionData.Rounding) != transactionData.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (transactionData.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, transactionData.Amount) });

            foreach (var item in paymentData.PaymentDetails)
            {
                sumPayDetail += item.Amount;
            }
            if (transactionData.Amount != sumPayDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de detalle de pago no coinciden") });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == transactionData.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                           x.PayMethodId == transactionData.PayMethodId &&
                                                          (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                                .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x =>
            {
                _saldo += x.Sign ? x.Total : x.Total * -1;
            });

            //Cancelación
            //var cancelacion = await _context.Transactions.Where(x => x.Folio == transactionData.Folio).FirstAsync();
            if (string.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

            var cancelacionPrevia = await _context.Transactions
                                                 .Include(x => x.TransactionFolios)
                                                 .Where(x => x.CancellationFolio == transactionData.Folio).FirstOrDefaultAsync();
            if (cancelacionPrevia != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago ha sido cancelado previamente. Folio->{0}", cancelacionPrevia.TransactionFolios.FirstOrDefault().Folio) });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    // fecha de transacción anterior
                    transaction.DateTransaction = transactionData.DateTransaction.AddMinutes(5);
                    transaction.Sign = false;
                    transaction.Amount = transactionData.Amount;
                    transaction.Tax = transactionData.Tax;
                    transaction.Rounding = transactionData.Rounding;
                    transaction.Total = transactionData.Total;
                    transaction.Aplication = transactionData.Aplication;
                    transaction.TypeTransactionId = 4;
                    transaction.PayMethodId = transactionData.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = transactionData.Folio;
                    transaction.AuthorizationOriginPayment = transactionData.AuthorizationOriginPayment;
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(transactionData.OriginPaymentId).ConfigureAwait(false);
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(transactionData.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.Account = null;
                    transaction.AccountNumber = null;
                    transaction.NumberBank = null;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    //se modifica estado de pago
                    //payment = await _context.Payments.FindAsync(pCancelPayment.Payment.Id);
                    paymentData.Status = "EP002";
                    _context.Entry(paymentData).State = EntityState.Modified;
                    await _context.SaveChangesAsync();




                    #region Codigo que agrege para Cancelar un pago de servicio

                    //Se modifican los debt
                    var debtList = paymentData.PaymentDetails.Select(x => x.DebtId).Distinct();

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
                                if (statusDebt.Count >= 1)
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


                            paymentData.PaymentDetails.ToList().ForEach(x =>
                            {
                                if (x.DebtId == item)
                                    sumPaymentDetails += x.Amount;
                            });

                            debt.Status = statusAnterior;
                            debt.OnAccount = debt.OnAccount - sumPaymentDetails;

                            _context.Entry(debt).State = EntityState.Modified;        //<-----------------------------------------------------------
                            await _context.SaveChangesAsync();                        //<-----------------------------------------------------------

                            bool removeItem = false;
                            if (!removeItem)
                            {
                                _context.Entry(await _context.DebtStatuses.FindAsync(statusDebt.First().Id)).State = EntityState.Deleted;
                                await _context.SaveChangesAsync();
                                removeItem = true;
                            }
                            removeItem = false;
                        }
                    }
                    else
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No es posible revertir deuda sin detalle de pago") });

                    //Se agregan los detalles para la transation de cancelacion.
                    foreach (var pay in paymentData.PaymentDetails)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pay.CodeConcept;
                        transactionDetail.Amount = pay.Amount;
                        transactionDetail.Description = pay.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);       //<-----------------------------------------------------------
                        await _context.SaveChangesAsync();                        //<----------------------------------------------------------- 

                        DebtDetail debtDetail = new DebtDetail();
                        debtDetail = await _context.DebtDetails.Where(x => x.DebtId == pay.DebtId &&
                                                                           x.CodeConcept == pay.CodeConcept).FirstAsync();

                        //Se resta en el debt_detail el monto de pago cancelado.
                        if (debtDetail.OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        debtDetail.OnAccount -= pay.Amount;
                        await _context.SaveChangesAsync();                        //<-----------------------------------------------------------
                    }
                    #endregion


                    #region Codigo previo de Julio
                    ////await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    ////var orderList = paymentData.PaymentDetails.Select(x => x.OrderSaleId).Distinct();

                    ////foreach (var item in orderList)
                    ////{
                    ////    OrderSale order = await _context.OrderSales.FindAsync(item);
                    ////    order.OnAccount = (order.Amount - order.OnAccount);
                    ////    order.Status = "EOS01";
                    ////    _context.Entry(order).State = EntityState.Modified;
                    ////    _context.SaveChanges();
                    ////}

                    ////int position = 0;
                    ////List<OrderSaleDetail> saleDetails = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == paymentData.PaymentDetails.First().OrderSaleId).ToListAsync();
                    ////foreach (var pay in paymentData.PaymentDetails)
                    ////{
                    ////    TransactionDetail transactionDetail = new TransactionDetail();
                    ////    transactionDetail.CodeConcept = pay.CodeConcept;
                    ////    transactionDetail.Amount = transaction.Amount;
                    ////    transactionDetail.Description = pay.Description;
                    ////    transactionDetail.Transaction = transaction;
                    ////    _context.TransactionDetails.Add(transactionDetail);
                    ////    await _context.SaveChangesAsync();

                    ////    if (saleDetails[position].OnAccount - pay.Amount < 0)
                    ////        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                    ////    saleDetails[position].OnAccount -= pay.Amount;
                    ////    saleDetails[position].Tax = 0;
                    ////    await _context.SaveChangesAsync();
                    ////    position++;
                    ////}
                    #endregion

                    //Se resta el monto la cancelacion al cierre del dia correspondiente.
                    DAL.Models.Transaction trans = movimientosCaja.Where(x => x.TypeTransactionId == 6 && x.PayMethodId == transactionData.PayMethodId).FirstOrDefault();
                    trans.TransactionDetails = _context.TransactionDetails.Where(x => x.TransactionId == trans.Id).ToList();
                    trans.Amount = trans.Amount - transactionData.Amount;
                    trans.Tax = trans.Tax - transactionData.Tax;
                    trans.Total = trans.Total - transactionData.Total;
                    trans.TransactionDetails.Where(x => x.Description.Contains("Retiro") && x.CodeConcept == "6").FirstOrDefault().Amount = trans.Total;

                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();

                    //Se Cancela la factura ligada al pago.
                    RequestsAPI RequestsFacturama = new RequestsAPI("https://api.facturama.mx/");
                    if (paymentData.HaveTaxReceipt)
                    {
                        try
                        {
                            var key = paymentData.TaxReceipts.Where(x => x.Status == "ET001").FirstOrDefault();
                            if (!string.IsNullOrEmpty(key.IdXmlFacturama))
                            {
                                var resultado = await RequestsFacturama.SendURIAsync(string.Format("api-lite/cfdis/{0}", key.IdXmlFacturama), HttpMethod.Delete, "gfdsystems", "gfds1st95");
                                var cfdiCancel = JsonConvert.DeserializeObject<RepuestaCancelacion>(resultado);
                                Byte[] bytes = Convert.FromBase64String(cfdiCancel.AcuseXmlBase64);
                                TaxReceiptCancel cancel = new TaxReceiptCancel
                                {
                                    CancelationDate = cfdiCancel.CancelationDate,
                                    AcuseXml = bytes,
                                    Message = cfdiCancel.Message,
                                    Status = cfdiCancel.Status,
                                    RequestDateCancel = cfdiCancel.RequestDate
                                };
                                TaxReceipt receipt = paymentData.TaxReceipts.Where(x => x.Status == "ET001").FirstOrDefault();
                                if (receipt != null)
                                {
                                    receipt.Status = "ET002";
                                    cancel.TaxReceipt = receipt;
                                    cancel.TaxReceiptId = receipt.Id;
                                    receipt.TaxReceiptCancels.Add(cancel);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar la factura electrónica" });
                        }
                    }
                    if (CancelSAC)
                    {
                        try
                        {
                            string error = string.Empty;
                            var payments = _context.AccountingPayments.Where(x => x.PaymentId == paymentData.Id && x.Status == "SAC03").OrderBy(x => x.Secuential).ToList();
                            foreach (var item in payments)
                            {
                                using (var command = _context.Database.GetDbConnection().CreateCommand())
                                {
                                    command.CommandText = "[dbo].[AccountingSAC]";
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add(new SqlParameter("@idAccountingPayment", item.Id));
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@Error",
                                        DbType = DbType.String,
                                        Size = 200,
                                        Direction = ParameterDirection.Output
                                    });
                                    this._context.Database.OpenConnection();
                                    using (var result = await command.ExecuteReaderAsync())
                                    {
                                        error += !string.IsNullOrEmpty(command.Parameters["@error"].Value.ToString()) ? command.Parameters["@error"].Value.ToString() + " -- " : "";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(error))
                            {
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar el pago en SAC " + error }); 
                            }
                        }
                        catch (Exception e)
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar el pago en SAC " + e.Message });
                        }

                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = TransactionId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok();
        }

        [HttpPost("SuperOrders/Cancel/{TransactionId}/{CancelSAC?}")]
        public async Task<IActionResult> TransactionOrderCancel([FromRoute] int TransactionId, [FromQuery] bool CancelSAC = false)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            Prepaid prepaid;
            decimal _sumTransactionDetail = 0;
            decimal sumPayDetail = 0;
            decimal _saldo = 0;
            string message = string.Empty;
            #region Validación
            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transactionData = await _context.Transactions
                                        .Include(x => x.TransactionFolios)
                                        .Include(t => t.TransactionDetails)
                                        .Where(x => x.Id == TransactionId).SingleOrDefaultAsync();

            var paymentData = await _context.Payments
                                            .Include(x => x.PaymentDetails)
                                            .Include(t => t.TaxReceipts)//then include cancelaciones
                                            .Where(x => x.TransactionFolio == transactionData.Folio)
                                            .SingleOrDefaultAsync();

            if (transactionData.Amount <= 0)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cancelar incorrecto") });

            if (String.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

            foreach (var item in transactionData.TransactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((transactionData.Amount + transactionData.Tax + transactionData.Rounding) != transactionData.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (transactionData.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, transactionData.Amount) });

            foreach (var item in paymentData.PaymentDetails)
            {
                sumPayDetail += item.Amount;
            }
            if (transactionData.Amount != sumPayDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de detalle de pago no coinciden") });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == transactionData.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                           x.PayMethodId == transactionData.PayMethodId &&
                                                          (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                                .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x =>
            {
                _saldo += x.Sign ? x.Total : x.Total * -1;
            });

            //Cancelación
            //var cancelacion = await _context.Transactions.Where(x => x.Folio == transactionData.Folio).FirstAsync();
            if (string.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

            var cancelacionPrevia = await _context.Transactions
                                                 .Include(x => x.TransactionFolios)
                                                 .Where(x => x.CancellationFolio == transactionData.Folio).FirstOrDefaultAsync();
            if (cancelacionPrevia != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago ha sido cancelado previamente. Folio->{0}", cancelacionPrevia.TransactionFolios.FirstOrDefault().Folio) });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    // fecha de transacción anterior
                    transaction.DateTransaction = transactionData.DateTransaction.AddMinutes(5);
                    transaction.Sign = false;
                    transaction.Amount = transactionData.Amount;
                    transaction.Tax = transactionData.Tax;
                    transaction.Rounding = transactionData.Rounding;
                    transaction.Total = transactionData.Total;
                    transaction.Aplication = transactionData.Aplication;
                    transaction.TypeTransactionId = 4;
                    transaction.PayMethodId = transactionData.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = transactionData.Folio;
                    transaction.AuthorizationOriginPayment = transactionData.AuthorizationOriginPayment;
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(transactionData.OriginPaymentId).ConfigureAwait(false);
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(transactionData.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.Account = null;
                    transaction.AccountNumber = null;
                    transaction.NumberBank = null;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    //se modifica estado de pago
                    //payment = await _context.Payments.FindAsync(pCancelPayment.Payment.Id);
                    paymentData.Status = "EP002";
                    _context.Entry(paymentData).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    var orderList = paymentData.PaymentDetails.Select(x => x.OrderSaleId).Distinct();

                    foreach (var item in orderList)
                    {
                        OrderSale order = await _context.OrderSales.FindAsync(item);
                        order.OnAccount = (order.Amount - order.OnAccount);
                        order.Status = "EOS01";
                        _context.Entry(order).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    int position = 0;
                    List<OrderSaleDetail> saleDetails = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == paymentData.PaymentDetails.First().OrderSaleId).ToListAsync();
                    foreach (var pay in paymentData.PaymentDetails)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pay.CodeConcept;
                        transactionDetail.Amount = transaction.Amount;
                        transactionDetail.Description = pay.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();

                        if (saleDetails[position].OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        saleDetails[position].OnAccount -= pay.Amount;
                        saleDetails[position].Tax = 0;
                        await _context.SaveChangesAsync();
                        position++;
                    }
                    
                    //Se resta el monto la cancelacion al cierre del dia correspondiente.
                    DAL.Models.Transaction trans = movimientosCaja.Where(x => x.TypeTransactionId == 6 && x.PayMethodId == transactionData.PayMethodId).FirstOrDefault();
                    trans.TransactionDetails = _context.TransactionDetails.Where(x => x.TransactionId == trans.Id).ToList();
                    trans.Amount = trans.Amount - transactionData.Amount;
                    trans.Tax = trans.Tax - transactionData.Tax;
                    trans.Total = trans.Total - transactionData.Total;
                    trans.TransactionDetails.Where(x => x.Description.Contains("Retiro") && x.CodeConcept == "6").FirstOrDefault().Amount = trans.Total;

                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();

                    //Se Cancela la factura ligada al pago.
                    RequestsAPI RequestsFacturama = new RequestsAPI("https://api.facturama.mx/");
                    if (paymentData.HaveTaxReceipt)
                    {
                        try
                        {
                            var key = paymentData.TaxReceipts.Where(x => x.Status == "ET001").FirstOrDefault();
                            if (key != null)
                            {
                                if (!string.IsNullOrEmpty(key.IdXmlFacturama))
                                {
                                    var resultado = await RequestsFacturama.SendURIAsync(string.Format("api-lite/cfdis/{0}", key.IdXmlFacturama), HttpMethod.Delete, "gfdsystems", "gfds1st95");
                                    var cfdiCancel = JsonConvert.DeserializeObject<RepuestaCancelacion>(resultado);
                                    Byte[] bytes = Convert.FromBase64String(cfdiCancel.AcuseXmlBase64);
                                    TaxReceiptCancel cancel = new TaxReceiptCancel
                                    {
                                        CancelationDate = cfdiCancel.CancelationDate,
                                        AcuseXml = bytes,
                                        Message = cfdiCancel.Message,
                                        Status = cfdiCancel.Status,
                                        RequestDateCancel = cfdiCancel.RequestDate
                                    };
                                    TaxReceipt receipt = paymentData.TaxReceipts.Where(x => x.Status == "ET001").FirstOrDefault();
                                    if (receipt != null)
                                    {
                                        receipt.Status = "ET002";
                                        cancel.TaxReceipt = receipt;
                                        cancel.TaxReceiptId = receipt.Id;
                                        receipt.TaxReceiptCancels.Add(cancel);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    message = "El pago no cuenta con un timbre en FACTURAMA, Favor de verificar";
                                }
                            }
                            if(key == null)
                            {
                                message = "El pago de cancelo pero la factura ya se encuentra en estatus cancelado";
                            }
                        }
                        catch (Exception)
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar la factura electrónica" });
                        }
                    }
                    if (CancelSAC)
                    {
                        try
                        {
                            string error = string.Empty;
                            var payments = _context.AccountingPayments.Where(x => x.PaymentId == paymentData.Id && x.Status == "SAC03").OrderBy(x => x.Secuential).ToList();
                            foreach (var item in payments)
                            {
                                using (var command = _context.Database.GetDbConnection().CreateCommand())
                                {
                                    command.CommandText = "[dbo].[AccountingSAC]";
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add(new SqlParameter("@idAccountingPayment", item.Id));
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@Error",
                                        DbType = DbType.String,
                                        Size = 200,
                                        Direction = ParameterDirection.Output
                                    });
                                    this._context.Database.OpenConnection();
                                    using (var result = await command.ExecuteReaderAsync())
                                    {
                                        error += !string.IsNullOrEmpty(command.Parameters["@error"].Value.ToString()) ? command.Parameters["@error"].Value.ToString() + " -- " : "";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(error))
                            {
                                message = error;
                            }
                        }
                        catch (Exception e)
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar el pago en SAC " + e.Message });
                        }
                      
                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = TransactionId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok(message);
        }

        [HttpPost("SuperOrders/CancelWithoutCFDI/{TransactionId}")]
        public async Task<IActionResult> TransactionOrderCancelWithoutCFDI([FromRoute] int TransactionId)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            Prepaid prepaid;
            decimal _sumTransactionDetail = 0;
            decimal sumPayDetail = 0;
            decimal _saldo = 0;
            string message = string.Empty;
            #region Validación
            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transactionData = await _context.Transactions
                                        .Include(x => x.TransactionFolios)
                                        .Include(t => t.TransactionDetails)
                                        .Where(x => x.Id == TransactionId).SingleOrDefaultAsync();

            var paymentData = await _context.Payments
                                            .Include(x => x.PaymentDetails)
                                            .Include(t => t.TaxReceipts)//then include cancelaciones
                                            .Where(x => x.TransactionFolio == transactionData.Folio)
                                            .SingleOrDefaultAsync();

            if (transactionData.Amount <= 0)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cancelar incorrecto") });

            if (String.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

            foreach (var item in transactionData.TransactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((transactionData.Amount + transactionData.Tax + transactionData.Rounding) != transactionData.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (transactionData.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, transactionData.Amount) });

            foreach (var item in paymentData.PaymentDetails)
            {
                sumPayDetail += item.Amount;
            }
            if (transactionData.Amount != sumPayDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de detalle de pago no coinciden") });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == transactionData.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                           x.PayMethodId == transactionData.PayMethodId &&
                                                          (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                                .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x =>
            {
                _saldo += x.Sign ? x.Total : x.Total * -1;
            });

            //Cancelación
            //var cancelacion = await _context.Transactions.Where(x => x.Folio == transactionData.Folio).FirstAsync();
            if (string.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

            var cancelacionPrevia = await _context.Transactions
                                                 .Include(x => x.TransactionFolios)
                                                 .Where(x => x.CancellationFolio == transactionData.Folio).FirstOrDefaultAsync();
            if (cancelacionPrevia != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago ha sido cancelado previamente. Folio->{0}", cancelacionPrevia.TransactionFolios.FirstOrDefault().Folio) });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    // fecha de transacción anterior
                    transaction.DateTransaction = transactionData.DateTransaction.AddMinutes(5);
                    transaction.Sign = false;
                    transaction.Amount = transactionData.Amount;
                    transaction.Tax = transactionData.Tax;
                    transaction.Rounding = transactionData.Rounding;
                    transaction.Total = transactionData.Total;
                    transaction.Aplication = transactionData.Aplication;
                    transaction.TypeTransactionId = 4;
                    transaction.PayMethodId = transactionData.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = transactionData.Folio;
                    transaction.AuthorizationOriginPayment = transactionData.AuthorizationOriginPayment;
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(transactionData.OriginPaymentId).ConfigureAwait(false);
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(transactionData.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.Account = null;
                    transaction.AccountNumber = null;
                    transaction.NumberBank = null;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    //se modifica estado de pago
                    //payment = await _context.Payments.FindAsync(pCancelPayment.Payment.Id);
                    paymentData.Status = "EP002";
                    _context.Entry(paymentData).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    var orderList = paymentData.PaymentDetails.Select(x => x.OrderSaleId).Distinct();

                    foreach (var item in orderList)
                    {
                        OrderSale order = await _context.OrderSales.FindAsync(item);
                        order.OnAccount = (order.Amount - order.OnAccount);
                        order.Status = "EOS01";
                        _context.Entry(order).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    int position = 0;
                    List<OrderSaleDetail> saleDetails = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == paymentData.PaymentDetails.First().OrderSaleId).ToListAsync();
                    foreach (var pay in paymentData.PaymentDetails)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pay.CodeConcept;
                        transactionDetail.Amount = transaction.Amount;
                        transactionDetail.Description = pay.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);
                        await _context.SaveChangesAsync();

                        if (saleDetails[position].OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        saleDetails[position].OnAccount -= pay.Amount;
                        saleDetails[position].Tax = 0;
                        await _context.SaveChangesAsync();
                        position++;
                    }

                    //Se resta el monto la cancelacion al cierre del dia correspondiente.
                    DAL.Models.Transaction trans = movimientosCaja.Where(x => x.TypeTransactionId == 6 && x.PayMethodId == transactionData.PayMethodId).FirstOrDefault();
                    trans.TransactionDetails = _context.TransactionDetails.Where(x => x.TransactionId == trans.Id).ToList();
                    trans.Amount = trans.Amount - transactionData.Amount;
                    trans.Tax = trans.Tax - transactionData.Tax;
                    trans.Total = trans.Total - transactionData.Total;
                    trans.TransactionDetails.Where(x => x.Description.Contains("Retiro") && x.CodeConcept == "6").FirstOrDefault().Amount = trans.Total;

                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = TransactionId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok(message);
        }

        [HttpPost("SuperService/CancelWithoutCFDI/{TransactionId}/{CancelSAC?}")]
        public async Task<IActionResult> TransactionServiceCancelWithoutCFDI([FromRoute] int TransactionId, [FromQuery] bool CancelSAC = false)
        {
            DAL.Models.Transaction transaction = new DAL.Models.Transaction();
            bool _validation = false;
            Prepaid prepaid;
            decimal _sumTransactionDetail = 0;
            decimal sumPayDetail = 0;
            decimal _saldo = 0;

            #region Validación
            //Parametros
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transactionData = await _context.Transactions
                                        .Include(x => x.TransactionFolios)
                                        .Include(t => t.TransactionDetails)
                                        .Where(x => x.Id == TransactionId).SingleOrDefaultAsync();

            var paymentData = await _context.Payments
                                            .Include(x => x.PaymentDetails)
                                            .Include(t => t.TaxReceipts)//then include cancelaciones
                                            .Where(x => x.TransactionFolio == transactionData.Folio)
                                            .SingleOrDefaultAsync();

            if (transactionData.Amount <= 0)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cancelar incorrecto") });

            if (String.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Debe ingresar folio de cancelación") });

            foreach (var item in transactionData.TransactionDetails)
            {
                _sumTransactionDetail += item.Amount;
            }

            if ((transactionData.Amount + transactionData.Tax + transactionData.Rounding) != transactionData.Total)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "El monto total de la transacción no es correcto" });

            if (transactionData.Amount != _sumTransactionDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El detalle de transacción: {0}, no coincide con el total de la transacción: {1}", _sumTransactionDetail, transactionData.Amount) });

            foreach (var item in paymentData.PaymentDetails)
            {
                sumPayDetail += item.Amount;
            }
            if (transactionData.Amount != sumPayDetail)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Los montos de detalle de pago no coinciden") });

            //Terminal
            TerminalUser terminalUser = new TerminalUser();
            terminalUser = await _context.TerminalUsers
                                             .Include(x => x.Terminal)
                                             .Include(x => x.User)
                                             .Where(x => x.Id == transactionData.TerminalUserId).FirstOrDefaultAsync();

            if (terminalUser == null)
                return NotFound();

            var movimientosCaja = await _context.Transactions
                                                .Include(x => x.TypeTransaction)
                                                .Where(x => x.TerminalUser.Id == terminalUser.Id &&
                                                           x.PayMethodId == transactionData.PayMethodId &&
                                                          (x.TypeTransactionId == 3 || x.TypeTransactionId == 4 || x.TypeTransactionId == 6))
                                                .OrderBy(x => x.Id).ToListAsync();

            if (movimientosCaja == null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Método de pago improcedente" });

            movimientosCaja.ForEach(x =>
            {
                _saldo += x.Sign ? x.Total : x.Total * -1;
            });

            //Cancelación
            //var cancelacion = await _context.Transactions.Where(x => x.Folio == transactionData.Folio).FirstAsync();
            if (string.IsNullOrEmpty(transactionData.Folio))
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No existe el folio a cancelación") });

            var cancelacionPrevia = await _context.Transactions
                                                 .Include(x => x.TransactionFolios)
                                                 .Where(x => x.CancellationFolio == transactionData.Folio).FirstOrDefaultAsync();
            if (cancelacionPrevia != null)
                return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("El pago ha sido cancelado previamente. Folio->{0}", cancelacionPrevia.TransactionFolios.FirstOrDefault().Folio) });

            #endregion

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Transacción en caja
                    transaction.Folio = Guid.NewGuid().ToString("D");
                    // fecha de transacción anterior
                    transaction.DateTransaction = transactionData.DateTransaction.AddMinutes(5);
                    transaction.Sign = false;
                    transaction.Amount = transactionData.Amount;
                    transaction.Tax = transactionData.Tax;
                    transaction.Rounding = transactionData.Rounding;
                    transaction.Total = transactionData.Total;
                    transaction.Aplication = transactionData.Aplication;
                    transaction.TypeTransactionId = 4;
                    transaction.PayMethodId = transactionData.PayMethodId;
                    transaction.TerminalUser = terminalUser;
                    transaction.CancellationFolio = transactionData.Folio;
                    transaction.AuthorizationOriginPayment = transactionData.AuthorizationOriginPayment;
                    transaction.OriginPayment = await _context.OriginPayments.FindAsync(transactionData.OriginPaymentId).ConfigureAwait(false);
                    transaction.ExternalOriginPayment = await _context.ExternalOriginPayments.FindAsync(transactionData.ExternalOriginPaymentId).ConfigureAwait(false);
                    transaction.Account = null;
                    transaction.AccountNumber = null;
                    transaction.NumberBank = null;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    //se modifica estado de pago
                    //payment = await _context.Payments.FindAsync(pCancelPayment.Payment.Id);
                    paymentData.Status = "EP002";
                    _context.Entry(paymentData).State = EntityState.Modified;
                    await _context.SaveChangesAsync();




                    #region Codigo que agrege para Cancelar un pago de servicio

                    //Se modifican los debt
                    var debtList = paymentData.PaymentDetails.Select(x => x.DebtId).Distinct();

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
                                if (statusDebt.Count >= 1)
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


                            paymentData.PaymentDetails.ToList().ForEach(x =>
                            {
                                if (x.DebtId == item)
                                    sumPaymentDetails += x.Amount;
                            });

                            debt.Status = statusAnterior;
                            debt.OnAccount = debt.OnAccount - sumPaymentDetails;

                            _context.Entry(debt).State = EntityState.Modified;        //<-----------------------------------------------------------
                            await _context.SaveChangesAsync();                        //<-----------------------------------------------------------

                            bool removeItem = false;
                            if (!removeItem)
                            {
                                _context.Entry(await _context.DebtStatuses.FindAsync(statusDebt.First().Id)).State = EntityState.Deleted;
                                await _context.SaveChangesAsync();
                                removeItem = true;
                            }
                            removeItem = false;
                        }
                    }
                    else
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("No es posible revertir deuda sin detalle de pago") });

                    //Se agregan los detalles para la transation de cancelacion.
                    foreach (var pay in paymentData.PaymentDetails)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail();
                        transactionDetail.CodeConcept = pay.CodeConcept;
                        transactionDetail.Amount = pay.Amount;
                        transactionDetail.Description = pay.Description;
                        transactionDetail.Transaction = transaction;
                        _context.TransactionDetails.Add(transactionDetail);       //<-----------------------------------------------------------
                        await _context.SaveChangesAsync();                        //<----------------------------------------------------------- 

                        DebtDetail debtDetail = new DebtDetail();
                        debtDetail = await _context.DebtDetails.Where(x => x.DebtId == pay.DebtId &&
                                                                           x.CodeConcept == pay.CodeConcept).FirstAsync();

                        //Se resta en el debt_detail el monto de pago cancelado.
                        if (debtDetail.OnAccount - pay.Amount < 0)
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                        debtDetail.OnAccount -= pay.Amount;
                        await _context.SaveChangesAsync();                        //<-----------------------------------------------------------
                    }
                    #endregion


                    #region Codigo previo de Julio
                    ////await _context.Terminal.Include(x => x.BranchOffice).FirstOrDefaultAsync(y => y.Id == transaction.TerminalUser.Terminal.Id);

                    ////var orderList = paymentData.PaymentDetails.Select(x => x.OrderSaleId).Distinct();

                    ////foreach (var item in orderList)
                    ////{
                    ////    OrderSale order = await _context.OrderSales.FindAsync(item);
                    ////    order.OnAccount = (order.Amount - order.OnAccount);
                    ////    order.Status = "EOS01";
                    ////    _context.Entry(order).State = EntityState.Modified;
                    ////    _context.SaveChanges();
                    ////}

                    ////int position = 0;
                    ////List<OrderSaleDetail> saleDetails = await _context.OrderSaleDetails.Where(x => x.OrderSaleId == paymentData.PaymentDetails.First().OrderSaleId).ToListAsync();
                    ////foreach (var pay in paymentData.PaymentDetails)
                    ////{
                    ////    TransactionDetail transactionDetail = new TransactionDetail();
                    ////    transactionDetail.CodeConcept = pay.CodeConcept;
                    ////    transactionDetail.Amount = transaction.Amount;
                    ////    transactionDetail.Description = pay.Description;
                    ////    transactionDetail.Transaction = transaction;
                    ////    _context.TransactionDetails.Add(transactionDetail);
                    ////    await _context.SaveChangesAsync();

                    ////    if (saleDetails[position].OnAccount - pay.Amount < 0)
                    ////        return StatusCode((int)TypeError.Code.Conflict, new { Error = string.Format("Monto a cuenta del concepto: {0}, inválido", arg0: pay.Description) });

                    ////    saleDetails[position].OnAccount -= pay.Amount;
                    ////    saleDetails[position].Tax = 0;
                    ////    await _context.SaveChangesAsync();
                    ////    position++;
                    ////}
                    #endregion

                    //Se resta el monto la cancelacion al cierre del dia correspondiente.
                    DAL.Models.Transaction trans = movimientosCaja.Where(x => x.TypeTransactionId == 6 && x.PayMethodId == transactionData.PayMethodId).FirstOrDefault();
                    trans.TransactionDetails = _context.TransactionDetails.Where(x => x.TransactionId == trans.Id).ToList();
                    trans.Amount = trans.Amount - transactionData.Amount;
                    trans.Tax = trans.Tax - transactionData.Tax;
                    trans.Total = trans.Total - transactionData.Total;
                    trans.TransactionDetails.Where(x => x.Description.Contains("Retiro") && x.CodeConcept == "6").FirstOrDefault().Amount = trans.Total;

                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();

                    if (CancelSAC)
                    {
                        try
                        {
                            string error = string.Empty;
                            var payments = _context.AccountingPayments.Where(x => x.PaymentId == paymentData.Id && x.Status == "SAC03").OrderBy(x => x.Secuential).ToList();
                            foreach (var item in payments)
                            {
                                using (var command = _context.Database.GetDbConnection().CreateCommand())
                                {
                                    command.CommandText = "[dbo].[AccountingSAC]";
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add(new SqlParameter("@idAccountingPayment", item.Id));
                                    command.Parameters.Add(new SqlParameter
                                    {
                                        ParameterName = "@Error",
                                        DbType = DbType.String,
                                        Size = 200,
                                        Direction = ParameterDirection.Output
                                    });
                                    this._context.Database.OpenConnection();
                                    using (var result = await command.ExecuteReaderAsync())
                                    {
                                        error += !string.IsNullOrEmpty(command.Parameters["@error"].Value.ToString()) ? command.Parameters["@error"].Value.ToString() + " -- " : "";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(error))
                            {
                                return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar el pago en SAC " + error });
                            }
                        }
                        catch (Exception e)
                        {
                            return StatusCode((int)TypeError.Code.Conflict, new { Error = "Error al intentar cancelar el pago en SAC " + e.Message });
                        }

                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = TransactionId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para ejecutar la transacción" });
            }

            return Ok();
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

        [HttpPost("getMovimintosFromCorte/{terminalUserId}")]
        public async Task<IActionResult> GetMovimientosFromCorte([FromRoute] int terminalUserId)
        {
            List<DAL.Models.Transaction> transactions = null;


            try
            {
                transactions = _context.Transactions
                                  .Where(x => x.TerminalUserId == terminalUserId).ToList();
                transactions.ForEach(t =>
                {

                    t.PayMethod = _context.PayMethods.Where(pm => pm.Id == t.PayMethodId).FirstOrDefault();
                    t.TerminalUser = _context.TerminalUsers.Where(tu => tu.Id == t.TerminalUserId).FirstOrDefault();
                    t.TransactionFolios = _context.TransactionFolios.Where(tf => tf.TransactionId == t.Id).ToList();
                    t.Payment = _context.Payments.Where(p => p.TransactionFolio == t.Folio).FirstOrDefault();
                    if (t.Payment != null) {
                        var agreement = _context.Agreements.Where(x => x.Id == t.Payment.AgreementId).FirstOrDefault();
                        t.Account = null;
                        if (agreement != null)
                        {
                            var client = agreement.Clients;
                            if (client.Count > 0)
                            {
                                t.Account = client.First().Name;
                            }
                            
                        }
                    }


                });
            }
            catch (Exception e)
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Description = e.ToMessageAndCompleteStacktrace();
                systemLog.DateLog = DateTime.UtcNow.ToLocalTime();
                systemLog.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                systemLog.Action = this.ControllerContext.RouteData.Values["action"].ToString();
                systemLog.Parameter = terminalUserId.ToString();
                CustomSystemLog helper = new CustomSystemLog(_context);
                helper.AddLog(systemLog);
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = "Problemas para recuperar los movimientos" });

            }
            return Ok(transactions);
        }
    }
}
