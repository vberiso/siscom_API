using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class SyncDataMobileVM
    {
        public int IdDispatchOrder { get; set; }
        public int IdReconectionCost { get; set; }
        public List<PhotoSyncMobile> PhotoSyncMobiles { get; set; }
        public List<LocationSyncMobile> LocationSyncMobiles { get; set; }

    }
}
