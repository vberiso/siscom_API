using Siscom.Agua.DAL.Models;
using System.Collections.Generic;

namespace Siscom.Agua.Api.Model
{
    public class SyncDataMobileVM
    {
        public SyncDataMobileVM()
        {
            this.OrderWorkStatuses = new List<OrderWorkStatusVM>();
            this.PhotoSyncMobiles = new List<PhotoSyncMobile>();
            this.LocationSyncMobiles = new List<LocationSyncMobile>();
            this.ActivitySyncMobiles = new List<ActivitySyncMobile>();
            this.MaterialSyncMobiles = new List<MaterialSyncMobile>();
            this.InspectionSyncMobiles = new List<InspectionSyncMobile>();
            this.ValuesSyncMobiles = new List<ValuesSyncMobiles>();
        }

        public int IdDispatchOrder { get; set; }
        public int IdReconectionCost { get; set; }
        public string StatusOrderWork { get; set; }
        public string DateRealization { get; set; }
        public string OpeningCommentary { get; set; }
        public string FinalCommentary { get; set; }
        public string UserIdAPI { get; set; }
        public string Description { get; set; }
        public string ValveCondition { get; set; }
        public int idReasonCatalog { get; set; }
        public List<OrderWorkStatusVM> OrderWorkStatuses { get; set; }
        public List<PhotoSyncMobile> PhotoSyncMobiles { get; set; }
        public List<LocationSyncMobile> LocationSyncMobiles { get; set; }
        public List<ActivitySyncMobile> ActivitySyncMobiles { get; set; }
        public List<MaterialSyncMobile> MaterialSyncMobiles { get; set; }
        public List<InspectionSyncMobile> InspectionSyncMobiles { get; set; }
        public List<ValuesSyncMobiles> ValuesSyncMobiles { get; set; }
    }
}
