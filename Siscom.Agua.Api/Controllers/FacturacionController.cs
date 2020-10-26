using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Facturama;
using Facturama.Services;
using Siscom.Agua.DAL;
using Siscom.Agua.Api.Data;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Siscom.Agua.Api.Services.TimbradoTimbox.Utils;
using Siscom.Agua.DAL.Models;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Net.Http.Headers;
using System.Dynamic;
using System.Net;
using System.Globalization;
using Siscom.Agua.DAL.Models;


namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FacturacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        static HttpClient client;

        //Sandbox 
        TimboxSandboxWS.timbrado_cfdi33_portClient cliente_timbrar;
        TimboxSandboxWS.timbrar_cfdi_result responseWs = new TimboxSandboxWS.timbrar_cfdi_result();
        //Productivo
        TimboxWS.timbrado_cfdi33_portClient cliente_timbrarProd;
        TimboxWS.timbrar_cfdi_result responseWsProd = new TimboxWS.timbrar_cfdi_result();

        public FacturacionController(ApplicationDbContext context)
        {
            _context = context;
            cliente_timbrar = new TimboxSandboxWS.timbrado_cfdi33_portClient();
            cliente_timbrarProd = new TimboxWS.timbrado_cfdi33_portClient();
        }

        // GET: api/OrderSales
        [HttpGet("ValidateFrom/{ini}/{fin}")]
        public async Task<IActionResult> GetOrderSales([FromRoute] string ini, [FromRoute] string fin)
        {
            DateTime FechaIni = new DateTime(int.Parse(ini.Split("-")[0]), int.Parse(ini.Split("-")[1]), int.Parse(ini.Split("-")[2]));
            DateTime FechaFin = new DateTime(int.Parse(fin.Split("-")[0]), int.Parse(fin.Split("-")[1]), int.Parse(fin.Split("-")[2]), 23, 59, 59);
            var tmp = _context.TaxReceipts.Where(x => x.TaxReceiptDate > FechaIni && x.TaxReceiptDate < FechaFin && x.Status == "ET001" && x.IdXmlFacturama != "Timbox").ToList();

            RequestsAPI RequestsFacturama = new RequestsAPI("https://api.facturama.mx/");
            int count = 0, refresh = 0;
            try
            {                
                foreach (var item in tmp)
                {
                    Facturama.Models.Response.Cfdi cfdiGet = new Facturama.Models.Response.Cfdi();
                    if (!string.IsNullOrEmpty(item.IdXmlFacturama))
                    {
                        var resultado = await RequestsFacturama.SendURIAsync(string.Format("api-lite/cfdis/{0}", item.IdXmlFacturama), HttpMethod.Get, "gfdsystems", "gfds1st95");
                        cfdiGet = JsonConvert.DeserializeObject<Facturama.Models.Response.Cfdi>(resultado);
                    }                    

                    if (cfdiGet.Items == null)
                    {
                        item.Status = "ET003";
                        _context.Entry(item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        var tmpPay = _context.Payments.FirstOrDefault(p => p.Id == item.PaymentId);
                        tmpPay.HaveTaxReceipt = false;
                        _context.Entry(tmpPay).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        refresh++;
                    }
                    count++;
                }
            }
            catch(Exception ex)
            {                
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Problemas al consultar los registros {0}", ex.Message) });
            }
            return Ok(string.Format("Registros revisados: {0} de {1}, se actualizaron: {2}", count, tmp.Count, refresh ));
        }


        [HttpGet("Cancelaciones/{ini}/{fin}")]
        public async Task<IActionResult> GetCancelaciones([FromRoute] string ini, [FromRoute] string fin)
        {
            try
            {
                DateTime FechaIni = new DateTime(int.Parse(ini.Split("-")[0]), int.Parse(ini.Split("-")[1]), int.Parse(ini.Split("-")[2]));
                DateTime FechaFin = new DateTime(int.Parse(fin.Split("-")[0]), int.Parse(fin.Split("-")[1]), int.Parse(fin.Split("-")[2]), 23, 59, 59);
                var tmp = _context.TaxReceipts.Where(x => x.TaxReceiptDate > FechaIni && x.TaxReceiptDate < FechaFin && x.Status == "ET001" && x.PDFInvoce != null).ToList();

                if(tmp.Count == 0)
                {
                    return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("No se encotraron facturas canceladas.") });
                }

                return Ok(tmp);                
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Error al tratar solicitar facturas canceladas.") });
            }
        }

        [HttpGet("Facturas/{ini}/{fin}/{id}/{status}")]
        public async Task<IActionResult> GetFacturasRango([FromRoute] string ini, [FromRoute] string fin, [FromRoute] string id, [FromRoute] string status)
        {
            try
            {
                DateTime FechaIni = new DateTime(int.Parse(ini.Split("-")[0]), int.Parse(ini.Split("-")[1]), int.Parse(ini.Split("-")[2]));
                DateTime FechaFin = new DateTime(int.Parse(fin.Split("-")[0]), int.Parse(fin.Split("-")[1]), int.Parse(fin.Split("-")[2]), 23, 59, 59);
                List<DAL.Models.TaxReceipt> tmp;
                if(status != "ET111")
                    tmp = _context.TaxReceipts.Where(x => x.TaxReceiptDate > FechaIni && x.TaxReceiptDate < FechaFin && x.Status == status && x.UserId == id && x.PDFInvoce != null).ToList();
                else
                    tmp = _context.TaxReceipts.Where(x => x.TaxReceiptDate >= FechaIni && x.TaxReceiptDate <= FechaFin && x.UserId == id && x.PDFInvoce != null).ToList();
                                          
                return Ok(tmp);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Error al tratar solicitar facturas canceladas.") });
            }
        }

        [HttpGet("FacturasFromDivision/{ini}/{fin}/{id}/{status}")]
        public async Task<IActionResult> GetFacturasFromDivision([FromRoute] string ini, [FromRoute] string fin, [FromRoute] string id, [FromRoute] string status)
        {
            try
            {
                DateTime FechaIni = new DateTime(int.Parse(ini.Split("-")[0]), int.Parse(ini.Split("-")[1]), int.Parse(ini.Split("-")[2]));
                DateTime FechaFin = new DateTime(int.Parse(fin.Split("-")[0]), int.Parse(fin.Split("-")[1]), int.Parse(fin.Split("-")[2]), 23, 59, 59);
                List<DAL.Models.TaxReceipt> tmp;

                string databaseName = _context.Database.GetDbConnection().Database;
                //Obtengo todos los pago en el rango de fechas                
                List<Payment> pagos = new List<Payment>();
                if ((databaseName.Contains("siscom_ayuntamiento") && int.Parse(id) == 6) || (databaseName.Contains("siscom_agua") && int.Parse(id) == 1)) //predial o sosapac
                {
                    pagos = await _context.Payments.Where(x => x.PaymentDate >= FechaIni && x.PaymentDate <= FechaFin && x.OrderSaleId == 0).ToListAsync();
                    List<int> idsPay = pagos.Select(x => x.Id).ToList();
                    tmp = _context.TaxReceipts.Where(x => x.Status == status && x.PDFInvoce != null && idsPay.Contains(x.PaymentId)).ToList();
                }
                else
                {
                    List<int> idsPaymentsTmps = new List<int>();
                    pagos = await _context.Payments.Where(x => x.PaymentDate >= FechaIni && x.PaymentDate <= FechaFin && x.OrderSaleId != 0).ToListAsync();
                    idsPaymentsTmps = pagos.Select(p => p.OrderSaleId).ToList();
                    List<OrderSale> orderSales = await _context.OrderSales.Where(o => idsPaymentsTmps.Contains(o.Id) && o.DivisionId == int.Parse(id)).ToListAsync();
                    List<int> idsOS = orderSales.Select(x => x.Id).ToList();
                    idsPaymentsTmps = pagos.Where(p => idsOS.Contains(p.OrderSaleId)).Select(p => p.Id).ToList();
                    tmp = _context.TaxReceipts.Where(x => x.Status == status && x.PDFInvoce != null && idsPaymentsTmps.Contains(x.PaymentId)).ToList();
                }

                return Ok(tmp);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = string.Format("Error al tratar solicitar facturas canceladas.") });
            }
        }

        /// <summary>
        /// //Facturacion con Timbox
        /// </summary>
        /// 
        [HttpGet("NumeroCertificado")]
        public async Task<IActionResult> GetNumeroCertificado()
        {
            string numeroCertificado, aa, b, c;
            FilesTimbrado FT = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".cer"));
            if(FT != null && string.IsNullOrEmpty(FT.CertificateNumber))
            {
                //rutas de los cer y key de CSD                
                string pathCer = FT.PathFile + FT.NameFile;
                //string pathKey = @"C:\inetpub\wwwroot\api\ccds\CSD_Escuela_Kemper_Urgate_EKU9003173C9.key";
                //string passKey = "12345678a";

                SelloDigital.leerCER(pathCer, out aa, out b, out c, out numeroCertificado);
                FT.CertificateNumber = numeroCertificado;

                _context.Update(FT);
                _context.SaveChanges();
            }
            
            return Ok(FT.CertificateNumber);
        }

        [HttpGet("DataUser")]
        public async Task<IActionResult> GetDataUser()
        {
            var user = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("USERTIMBOX") && s.IsActive == true);
            var pass = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("PASSTIMBOX") && s.IsActive == true);
            var definition = new { User = user.TextColumn, Pass = pass.TextColumn};
            return Ok(definition);
        }

        private string ObtenerNumeroCertificado()
        {
            string numeroCertificado, aa, b, c;
            FilesTimbrado FT = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".cer"));
            if (FT != null && string.IsNullOrEmpty(FT.CertificateNumber))
            {
                //rutas de los cer y key de CSD                
                string pathCer = FT.PathFile + FT.NameFile;                
                SelloDigital.leerCER(pathCer, out aa, out b, out c, out numeroCertificado);
                FT.CertificateNumber = numeroCertificado;

                _context.Update(FT);
                _context.SaveChanges();
            }

            return FT.CertificateNumber;
        }

        private string CreateXML(Comprobante pComprobante, string nombreXml, out string rutaFile)
        {            
            string pathXML;
            try
            {                
                string path = rutaDescaga();
                pathXML = string.Format("{0}\\{1}", path, nombreXml);
                rutaFile = pathXML;

                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
                xmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xmlSerializerNamespaces.Add("tfd", "http://www.sat.gob.mx/timbrefiscaldigital");
                
                //SERIALIZAMOS.------------------------------------------------- 
                XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));

                string sXml = "";
                using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
                {
                    using (XmlWriter writter = XmlWriter.Create(sww))
                    {
                        oXmlSerializar.Serialize(writter, pComprobante, xmlSerializerNamespaces);
                        sXml = sww.ToString();
                    }
                }

                //guardamos el string en un archivo
                System.IO.File.WriteAllText(pathXML, sXml);
                return sXml;
            }
            catch (Exception ex)
            {
                rutaFile = "";
                return "";
            }
        }

        private string rutaDescaga()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Facturas";            
            try
            {
                DirectoryInfo di;
                if (!Directory.Exists(path))
                {
                    di = Directory.CreateDirectory(path);
                }
                //Se crea la una nueva carpeta por fecha
                path = path + "\\Facturas" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(path))
                {
                    di = Directory.CreateDirectory(path);
                }
                return path + "\\";
            }
            catch (Exception ex)
            {
                path = @"C:\Facturas\";
                DirectoryInfo di;
                if (!Directory.Exists(path))
                {
                    di = Directory.CreateDirectory(path);
                }
                return path + "\\";
            }
        }

        [HttpPost("SellarXml")]
        public async Task<IActionResult> getSellarXml([FromBody]Comprobante comprobante)
        {
            string rutaXML, rutaXMLFinal;
            try
            {                
                AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

                //Se obtiene la ubicacion de los archivos de facturacion del emisor de la factura.
                FilesTimbrado FTxslt = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".xslt"));
                FilesTimbrado FTcer = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".cer"));
                FilesTimbrado FTkey = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".key"));

                //Obtenemos el numero de certificado del .cer
                comprobante.NoCertificado = ObtenerNumeroCertificado();

                //Creamos el archivo xml.                
                string textXML = CreateXML(comprobante, string.Format("{0}_{1}_{2}_{3}.xml", "XmlPrev", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Folio), out rutaXML);

                //Se crea la cadena original
                string cadenaoriginal = "";
                XslCompiledTransform transformador = new XslCompiledTransform(true);
                transformador.Load(FTxslt.PathFile + FTxslt.NameFile);

                string nombreFile = string.Format("{0}_{1}_{2}_{3}.xml", "XmlPrev", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Folio);
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
                    {
                        transformador.Transform(rutaXML, xwo);
                        cadenaoriginal = sw.ToString();
                    }
                }

                SelloDigital SD = new SelloDigital();
                comprobante.Certificado = SD.Certificado(FTcer.PathFile + FTcer.NameFile);
                comprobante.Sello = SD.Sellar(cadenaoriginal, FTkey.PathFile + FTkey.NameFile, FTkey.PassKey);

                string textXMLFinal = CreateXML(comprobante, string.Format("{0}_{1}_{2}_{3}.xml", "Xml", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Serie + comprobante.Folio), out  rutaXMLFinal);

                //conversion de xml a base64
                XmlDocument doc_xml = new XmlDocument();
                doc_xml.Load(rutaXMLFinal);
                byte[] base64 = Encoding.UTF8.GetBytes(doc_xml.InnerXml);
                string convertXml = Convert.ToBase64String(base64);

                //Se eliminan los documentos temporales.
                if(System.IO.File.Exists(rutaXML))
                    System.IO.File.Delete(rutaXML);
                if (System.IO.File.Exists(rutaXMLFinal))
                    System.IO.File.Delete(rutaXMLFinal);

                return Ok(convertXml);
            }
            catch(Exception ex)
            {
                return StatusCode((int)TypeError.Code.InternalServerError, new { Error = ex.Message });
            }            
        }

        //Facturacion online
        [HttpPost("TimbrarTimbox/{pIdPayment}")]
        public async Task<IActionResult> getTimbrarTimbox([FromBody]Comprobante comprobante, [FromRoute]int pIdPayment)
        {
            string rutaXML, rutaXMLFinal;
            string resultadoFinal;
            XmlDocument xmlDocument;
            try
            {
                AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

                //Se obtiene la ubicacion de los archivos de facturacion del emisor de la factura.
                FilesTimbrado FTxslt = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".xslt"));
                FilesTimbrado FTcer = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".cer"));
                FilesTimbrado FTkey = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains(".key"));

                //Obtenemos el numero de certificado del .cer
                comprobante.NoCertificado = ObtenerNumeroCertificado();

                //Creamos el archivo xml.                
                string textXML = CreateXML(comprobante, string.Format("{0}_{1}_{2}_{3}.xml", "XmlPrev", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Folio), out rutaXML);

                //Se crea la cadena original
                string cadenaoriginal = "";
                XslCompiledTransform transformador = new XslCompiledTransform(true);
                transformador.Load(FTxslt.PathFile + FTxslt.NameFile);

                string nombreFile = string.Format("{0}_{1}_{2}_{3}.xml", "XmlPrev", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Folio);
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
                    {
                        transformador.Transform(rutaXML, xwo);
                        cadenaoriginal = sw.ToString();
                    }
                }

                SelloDigital SD = new SelloDigital();
                comprobante.Certificado = SD.Certificado(FTcer.PathFile + FTcer.NameFile);
                comprobante.Sello = SD.Sellar(cadenaoriginal, FTkey.PathFile + FTkey.NameFile, FTkey.PassKey);

                string textXMLFinal = CreateXML(comprobante, string.Format("{0}_{1}_{2}_{3}.xml", "Xml", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Serie + comprobante.Folio), out rutaXMLFinal);

                //conversion de xml a base64
                XmlDocument doc_xml = new XmlDocument();
                doc_xml.Load(rutaXMLFinal);
                byte[] base64 = Encoding.UTF8.GetBytes(doc_xml.InnerXml);
                string converB64tXml = Convert.ToBase64String(base64);

                //Se obtienen los datos de autenticacion
                var user = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("USERTIMBOX") && s.IsActive == true);
                var pass = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("PASSTIMBOX") && s.IsActive == true);
                if (user != null && pass != null)
                {
                    var ambiente = _context.SystemParameters.FirstOrDefault(x => x.Name.Contains("CFDITEST"));
                    if (ambiente.TextColumn.Contains("F") || ambiente.TextColumn.Contains("f"))
                    {
                        responseWsProd = await cliente_timbrarProd.timbrar_cfdiAsync(user.TextColumn, pass.TextColumn, converB64tXml);
                        resultadoFinal = responseWsProd.xml; //.Replace("\n", "").Replace("\\\"", "\"");
                    }
                    else
                    {
                        responseWs = await cliente_timbrar.timbrar_cfdiAsync(user.TextColumn, pass.TextColumn, converB64tXml);
                        resultadoFinal = responseWs.xml; //.Replace("\n", "").Replace("\\\"", "\"");
                    }

                    //Se genera xml para salvarlo en disco.
                    string path = rutaDescaga();
                    string nombreXML = string.Format("\\{0}_{1}_{2}.xml", comprobante.Emisor.Rfc, comprobante.Receptor.Rfc, comprobante.Serie + comprobante.Folio);
                    xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(resultadoFinal);
                    xmlDocument.Save(path + nombreXML);

                    //Se rescata el UUID del sat.                   
                    XmlNodeList elemList = xmlDocument.GetElementsByTagName("tfd:TimbreFiscalDigital");                    
                    string Uuid = elemList.Item(0).Attributes["UUID"].Value;
                    
                    var userOnline = _context.Users.FirstOrDefault(x => x.Name.Contains("Online") && x.IsActive == true);

                    //Se guarda registro en BD
                    TaxReceipt xMLS = new TaxReceipt();
                    xMLS.TaxReceiptDate = DateTime.Now;
                    xMLS.XML = xmlDocument.OuterXml;
                    xMLS.FielXML = Uuid;
                    xMLS.RFC = comprobante.Receptor.Rfc;
                    xMLS.Type = "CAT03";
                    xMLS.Status = "ET001";
                    xMLS.PaymentId = pIdPayment;
                    xMLS.UserId = userOnline != null ? userOnline.Id : "378c0632-2e7c-4d0f-b5b5-8821d21bd2f0";
                    xMLS.IdXmlFacturama = "Timbox";
                    xMLS.UsoCFDI = comprobante.Receptor.UsoCFDI;
                    _context.TaxReceipts.Add(xMLS);
                    _context.SaveChanges();
                }
                else
                {
                    return StatusCode((int)TypeError.Code.Conflict , new { Error = "Sin usuario de autenticación a Timbox." });
                }

                //Se eliminan los documentos temporales.
                if (System.IO.File.Exists(rutaXML))
                    System.IO.File.Delete(rutaXML);
                if (System.IO.File.Exists(rutaXMLFinal))
                    System.IO.File.Delete(rutaXMLFinal);

                var fromXml = JsonConvert.SerializeXmlNode(xmlDocument);                
                return Ok(fromXml);
            }
            catch (Exception ex)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = ex.Message });
            }
        }

        private string ActualizaRegistros(List<Siscom.Agua.Api.Model.FolioCancelacion> pFC, TimboxCancelacionWS.cancelar_cfdi_result response)
        {
            XmlDocument foliosCancelacion = new XmlDocument();
            foliosCancelacion.LoadXml(response.folios_cancelacion);
            XmlNodeList codigo = foliosCancelacion.GetElementsByTagName("codigo");
            if(codigo[0].InnerXml.ToString().Contains("201"))
            {
                foreach (var folio in pFC)
                {
                    var tax = _context.TaxReceipts.Find(folio.TaxReceiptId);
                    if (tax != null)
                    {
                        //Se cambia el status del tax receipt.
                        tax.Status = "ET002";
                        _context.Update(tax);
                        _context.SaveChanges();

                        Encoding encoding = Encoding.UTF8;
                        //Se guarda acuse de cancelacion
                        TaxReceiptCancel taxReceiptCancel = new TaxReceiptCancel();
                        taxReceiptCancel.Status = "Canceled";
                        taxReceiptCancel.Message = "La factura se ha cancelado existosamente";
                        taxReceiptCancel.RequestDateCancel = DateTime.Now;
                        taxReceiptCancel.CancelationDate = DateTime.Now;
                        taxReceiptCancel.AcuseXml = encoding.GetBytes(response.acuse_cancelacion);
                        taxReceiptCancel.TaxReceiptId = folio.TaxReceiptId;
                        _context.TaxReceiptCancels.Add(taxReceiptCancel);
                        _context.SaveChanges();
                    }
                }
            }
            return codigo[0].InnerXml.ToString();
        }

        //Cancelar facturas de timbox
        [HttpPost("CancelarFacturasTimbox")]
        public async Task<IActionResult> getCancelarFacturasTimbox([FromBody]List<Siscom.Agua.Api.Model.FolioCancelacion> foliosCancelacion)
        {
            try
            {
                //Se obtiene la ubicacion de los archivos de facturacion del emisor de la factura.
                FilesTimbrado CerPem = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains("cer.pem"));
                FilesTimbrado KeyPem = _context.FilesTimbrados.FirstOrDefault(f => f.IsActive == true && f.NameFile.Contains("key.pem"));

                //Obtengo los archivos pem.
                string file_cer_pem = System.IO.File.ReadAllText(CerPem.PathFile + CerPem.NameFile);
                string file_key_pem = System.IO.File.ReadAllText(KeyPem.PathFile + KeyPem.NameFile);

                //Obtengo la base actual.
                var RFCActual = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("RFC"));

                //Se obtienen los datos de autenticacion
                var user = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("USERTIMBOX") && s.IsActive == true);
                var pass = _context.SystemParameters.FirstOrDefault(s => s.Name.Contains("PASSTIMBOX") && s.IsActive == true);

                var ambiente = _context.SystemParameters.FirstOrDefault(x => x.Name.Contains("CFDITEST"));
                if (ambiente.TextColumn.Contains("F") || ambiente.TextColumn.Contains("f"))   //Produccion
                {
                    TimboxCancelacionWS.folios folios_datos = new TimboxCancelacionWS.folios();
                    var lista_folios = new List<TimboxCancelacionWS.folio>();
                    foreach (var i in foliosCancelacion)
                        lista_folios.Add(new TimboxCancelacionWS.folio { uuid = i.UUID, rfc_receptor = i.ReceptorRFC, total = i.Total.ToString() });
                    folios_datos.folio = lista_folios.ToArray();

                    TimboxCancelacionWS.cancelacion_portClient clienteCancelacion = new TimboxCancelacionWS.cancelacion_portClient();
                    TimboxCancelacionWS.cancelar_cfdi_result response = new TimboxCancelacionWS.cancelar_cfdi_result();

                    response = await clienteCancelacion.cancelar_cfdiAsync(user.TextColumn, pass.TextColumn, RFCActual.TextColumn, folios_datos, file_cer_pem, file_key_pem);

                    string resAct = ActualizaRegistros(foliosCancelacion, response);

                    if (resAct.Contains("201") || resAct.Contains("202"))
                    {
                        XmlDocument acuse_cancelacion = new XmlDocument();
                        acuse_cancelacion.LoadXml(response.folios_cancelacion);
                        return Ok(response.folios_cancelacion.ToString());
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.BadRequest, new { error = "No fue posible cancelar operación." });
                    }
                    
                }
                else                                                                          //Desarrollo
                {
                    TimboxCancelacionSandboxWS.folios folios_datos = new TimboxCancelacionSandboxWS.folios();
                    var lista_folios = new List<TimboxCancelacionSandboxWS.folio>();
                    foreach (var i in foliosCancelacion)
                        lista_folios.Add(new TimboxCancelacionSandboxWS.folio { uuid = i.UUID, rfc_receptor = i.ReceptorRFC, total = i.Total.ToString() });
                    folios_datos.folio = lista_folios.ToArray();

                    TimboxCancelacionSandboxWS.cancelacion_portClient clienteCancelacionSandbox = new TimboxCancelacionSandboxWS.cancelacion_portClient();
                    TimboxCancelacionSandboxWS.cancelar_cfdi_result response = new TimboxCancelacionSandboxWS.cancelar_cfdi_result();

                    response = await clienteCancelacionSandbox.cancelar_cfdiAsync(user.TextColumn, pass.TextColumn, RFCActual.TextColumn, folios_datos, file_cer_pem, file_key_pem);

                    XmlDocument acuse_cancelacion = new XmlDocument();
                    acuse_cancelacion.LoadXml(response.folios_cancelacion);

                    return Ok(response.folios_cancelacion.ToString());
                }                
            }
            catch(Exception ex)
            {
                return StatusCode((int)TypeError.Code.BadRequest, new { Error = ex.Message });
            }
        }


        //Cancelaciones en grupo para Facturama.
        //ej: idsPayments = '364501,365343,361740,362243,...'
        [HttpPost("CancelarFacturasFacturama")]
        public async Task<IActionResult> getCancelarFacturas([FromBody]string idsPayments)
        {
            try
            {
                client = new HttpClient();
                // Update port # in the following line.
                client.BaseAddress = new Uri("https://api.facturama.mx/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var byteArray = Encoding.ASCII.GetBytes("gfdsystems:gfds1st95");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var ids = idsPayments.Split(",");
                int TotalPeticiones = ids.Count(), TotalCanceladas = 0, TotalPendientes = 0;
                foreach(var id in ids)
                {
                    var tax = _context.TaxReceipts.FirstOrDefault(t => t.PaymentId == int.Parse(id));

                    if(tax != null && tax.Status == "ET001" && !string.IsNullOrEmpty(tax.IdXmlFacturama))
                    {
                        HttpResponseMessage response = await client.DeleteAsync($"api-lite/cfdis/{tax.IdXmlFacturama}");
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var definition = new { Status = "", Message = "", RequestDate = "", AcuseXmlBase64 = "", CancelationDate = "" };
                            var respCancel = JsonConvert.DeserializeAnonymousType(jsonString, definition);

                            if (respCancel.Status == "canceled")
                            {
                                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                                TaxReceiptCancel taxReceiptCancel = new TaxReceiptCancel()
                                {
                                    Status = respCancel.Status,
                                    Message = respCancel.Message,
                                    RequestDateCancel = DateTime.ParseExact(respCancel.RequestDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.CurrentCulture),
                                    CancelationDate = DateTime.ParseExact(respCancel.CancelationDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.CurrentCulture),
                                    //AcuseXml = respCancel.AcuseXmlBase64 == null ? null : encoding.GetBytes(respCancel.AcuseXmlBase64),
                                    TaxReceiptId = tax.Id
                                };
                                _context.TaxReceiptCancels.Add(taxReceiptCancel);
                                _context.SaveChanges();

                                tax.Status = "ET002";
                                _context.Update(tax);
                                _context.SaveChanges();

                                TotalCanceladas++;
                            }
                            else 
                            {
                                TotalPendientes++;
                            }                            
                        }
                    }                                       
                }

                return Ok( string.Format("Se cancelaron {0} de {1}, Pendientes: {2}", TotalCanceladas, TotalPeticiones, TotalPendientes));
            }
            catch(Exception ex)
            {
                return BadRequest( "Error" );
            }
        }

        //Obtiene los status de un grupo de facturas.
        [HttpGet("StatusFacturasFacturama")]
        public async Task<IActionResult> getStatusFacturas([FromBody]string idsFacturama)
        {
            try
            {
                client = new HttpClient();
                // Update port # in the following line.
                client.BaseAddress = new Uri("https://api.facturama.mx/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var byteArray = Encoding.ASCII.GetBytes("gfdsystems:gfds1st95");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var ids = idsFacturama.Split(",");
                int TotalPeticiones = ids.Count(), TotalCanceladas = 0, TotalPendientes = 0, TotalActivas = 0;
                List<Model.FacturaFacturamaStatus> lstfacturaFacturamaStatuses = new List<Model.FacturaFacturamaStatus>();
                foreach (var id in ids)
                {                    
                    HttpResponseMessage response = await client.GetAsync($"api-lite/cfdis/{id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var definition = new { Status = "", Id = "", Folio = "", Date = "", Total = "" };
                        var respStatus = JsonConvert.DeserializeAnonymousType(jsonString, definition);

                        Model.FacturaFacturamaStatus factura = new Model.FacturaFacturamaStatus()
                        {
                            Id = respStatus.Id,
                            Status = respStatus.Status,
                            Folio = respStatus.Folio,
                            Date = DateTime.ParseExact(respStatus.Date, "yyyy-MM-ddTHH:mm:ss", CultureInfo.CurrentCulture),
                            Total = decimal.Parse(respStatus.Total)
                        };
                        lstfacturaFacturamaStatuses.Add(factura);

                        if (respStatus.Status == "active")                        
                            TotalActivas++;                        
                        else if (respStatus.Status.Contains("ancel"))
                            TotalCanceladas++;
                        else                        
                            TotalPendientes++;
                        
                    }
                    
                }

                return Ok(lstfacturaFacturamaStatuses);
            }
            catch (Exception ex)
            {
                return BadRequest("Error");
            }
        }


    }
}