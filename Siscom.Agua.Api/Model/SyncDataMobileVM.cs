using System.Collections.Generic;

namespace Siscom.Agua.Api.Model
{
    public class SyncDataMobileVM
    {
        public int IdDispatchOrder { get; set; }
        public int IdReconectionCost { get; set; }
        public string StatusOrderWork { get; set; }
        public string DateRealization { get; set; }
        public string OpeningCommentary { get; set; }
        public string FinalCommentary { get; set; }
        public List<PhotoSyncMobile> PhotoSyncMobiles { get; set; }
        public List<LocationSyncMobile> LocationSyncMobiles { get; set; }

    }
}
