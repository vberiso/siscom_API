using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class SyncDataInspectionList
    {
        public SyncDataInspectionList()
        {
            this.PhotoSyncMobiles = new List<PhotoSyncMobile>();
            this.AnomalySyncMobiles = new List<AnomalySyncMobile>();
            this.OrderWorkStatuses = new List<OrderWorkStatusVM>();
        }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Comentary { get; set; }
        public string Observations { get; set; }
        public int AgreementId { get; set; }
        public string UserIdAPI { get; set; }
        public int IdDispatchOrder { get; set; }
        public bool HaveAnomaly { get; set; }
        public string Folio { get; set; }
        public bool CompleteList { get; set; }
        public List<PhotoSyncMobile> PhotoSyncMobiles { get; set; }
        public List<AnomalySyncMobile> AnomalySyncMobiles { get; set; }
        public List<OrderWorkStatusVM> OrderWorkStatuses { get; set; }
    }
}
