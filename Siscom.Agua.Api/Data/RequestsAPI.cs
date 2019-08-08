using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Data
{
    public class RequestsAPI
    {
        private string UrlBase;
        
        public RequestsAPI(string pUrlBase)
        {
            UrlBase = pUrlBase;
        }

        public async Task<string> SendURIAsync(string endPoint, HttpMethod method, string User, string Pass, HttpContent httpContent = null)
        {

            using (var client = new HttpClient())
            {
                HttpResponseMessage httpResponse = null;
                try
                {
                    if (method.Method == "GET")
                    {
                        HttpRequestMessage httpRequest = new HttpRequestMessage
                        {
                            Method = method,
                            RequestUri = new Uri(UrlBase + endPoint),
                        };
                        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var byteArray = Encoding.ASCII.GetBytes(User + ":" + Pass);
                        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                        httpResponse = await client.SendAsync(httpRequest);
                    }
                    else
                    {
                        HttpRequestMessage httpRequest = new HttpRequestMessage
                        {
                            Method = method,
                            RequestUri = new Uri(UrlBase + endPoint),
                            Content = httpContent,
                        };
                        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var byteArray = Encoding.ASCII.GetBytes(User + ":" + Pass);
                        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                        httpResponse = await client.SendAsync(httpRequest);
                    }

                    switch (httpResponse.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            return await httpResponse.Content.ReadAsStringAsync();
                        case System.Net.HttpStatusCode.InternalServerError:
                            return "{\"error\": \"Servicio temporalmente no disponible contacte al Administrador, disculpe las molestias: x000500\"}";
                        case System.Net.HttpStatusCode.ServiceUnavailable:
                            return "{\"error\": \"Servicio temporalmente no disponible contacte al Administrador, disculpe las molestias: x000503\"}";
                        case System.Net.HttpStatusCode.Forbidden:
                            return "{\"error\": \"Servicio temporalmente no disponible contacte al Administrador, disculpe las molestias: x000503\"}";
                        case System.Net.HttpStatusCode.GatewayTimeout:
                            return "{\"error\": \"Servicio temporalmente no disponible contacte al Administrador, disculpe las molestias: x000504\"}";
                        case System.Net.HttpStatusCode.Unauthorized:
                            return "{\"error\": \"Sesión expírada o no cuenta con la autorización: x000401 \"}";
                        default:
                            return await httpResponse.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception e)
                {
                    return "{\"error\": \"Servicio no disponible contacte al Administrador\"}";
                }
                finally
                {
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }
                }
            }
        }


    }
}
