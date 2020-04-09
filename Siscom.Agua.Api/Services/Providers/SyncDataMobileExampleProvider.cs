using Siscom.Agua.Api.Model;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Services.Providers
{
    public class SyncDataMobileExampleProvider : IExamplesProvider
    {
        public object GetExamples()
        {
            return new SyncDataMobileVM
            {
                DateRealization = "2020-04-09 01:10:15",
                IdDispatchOrder = 1,
                IdReconectionCost = 256,
                StatusOrderWork = "EOT02",
                LocationSyncMobiles = new List<LocationSyncMobile>
                {
                    new LocationSyncMobile
                    {
                        Latitud = "19.1195901",
                        Longitud = "-98.1704285",
                        Type = "OTML1"
                    }
                },
                PhotoSyncMobiles = new List<PhotoSyncMobile>
                {
                    new PhotoSyncMobile
                    {
                        Type = "OTMP1",
                        DateTake = "2020-04-09 01:10:15",
                        Photo = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAQAAADZc7J/AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADukAAA7pAQ4zQhwAAAAHdElNRQfkBAgTDS9xKiMmAAABdUlEQVRIx52Tv0sCYRyHHyWiQ4xA+iOExCFIgsAlGlscGwwHhxZB/xM318ipLTejqOBsadHoxxBBQbRlBIFD+mnI0+6X3t1z073weT/3Pvd9Y0QlRZESX1GiMfI0GSDEXdjwCmV6aPJUo/X2GCLEgFTQ81Z5QIg+dTYxEUNEM0h4nQbfCHFDmQQJLhAmt4h80PN+0iALMInnEI/Egvdiiy9T9xfo1euMG3wwYDVorzMO+4ijoL3uOHTsAmf1esXTdoHnk/+75iM0iYkwSY7fHQLbCNGlQDxQ3KDvnMBtrhHiiTILMz8eb4EAW5wixDMVlmbEXQLtm7QQ4oUKhk88PW8Cc5wwQrxRc5zdU6A3GQ75Qa52/CfQTZpLRM2xWgx6hf+2GPKOYVvrzL/C/zlGHIQR6CTLiFcWwwm000KUwgucsoF4Gk+nzwTOo43YiyLQIo+4J04mrMApV4hCFIEWO4iu+wqHwUTRBFrsomgCLWK0OfMT+AuReM8E3friwAAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAyMC0wNC0wOFQxOToxMzo0NyswMDowMOcpS3YAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjAtMDQtMDhUMTk6MTM6NDcrMDA6MDCWdPPKAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAABJRU5ErkJggg=="
                    }
                }
            };
        }
    }
}
