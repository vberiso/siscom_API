using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class PreAgreementVM
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public int TypeIntakeId { get; set; }
        public int TypeServiceId { get; set; }
        public int TypeUseId { get; set; }
        public int TypeClassificationId { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientSecondLastName { get; set; }
        public string Street { get; set; }
        public string Outdoor { get; set; }
        public string Indoor { get; set; }
        public string Zip { get; set; }
        public string Reference { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public int SuburbsId { get; set; }
        public int ServiceId1 { get; set; }
        public int ServiceId2 { get; set; }
        public int ServiceId3 { get; set; }
        public string RegistrationReason { get; set; }
        public string Observation { get; set; }
    }
}
