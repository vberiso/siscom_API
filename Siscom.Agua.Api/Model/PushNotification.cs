using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PushNotification
    {
        public string UserRequestId { get; set; }
        public string UserResponseId { get; set; }
        public string BranchOffice { get; set; }
        public int AuthorizationDiscountId { get; set; }
        public bool IsReply { get; set; }
        public string Observation { get; set; }
        public string Status { get; set; }
        public string Account { get; set; }
        public string AccountAdjusted { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsView { get; set; }
    }
}
