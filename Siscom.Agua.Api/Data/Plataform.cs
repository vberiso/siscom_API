using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Data
{
    public class Plataform
    {
        static IServiceProvider services = null;
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("");
                }
                services = value;
            }
        }

        public static bool IsAyuntamiento
        {
            get
            {
                IConfiguration configuration = services.GetService(typeof(IConfiguration)) as IConfiguration;
                var isAyuntamiento = bool.Parse(configuration.GetConnectionString("isAyuntamiento"));
                return isAyuntamiento;
            }
        }
    }
}
