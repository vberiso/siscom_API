using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class AgreementDataVM
    {
        public List<TypeService> TypeService { get; set; }
        public List<TypeUse> TypeUse { get; set; }
        public List<TypeConsume> TypeConsume { get; set; }
        public List<TypeRegime> TypeRegime { get; set; }
        public List<TypePeriod> TypePeriod { get; set; }
        public List<TypeCommercialBusiness> TypeCommertialBusiness { get; set; }
        public List<TypeStateService> TypeStateService { get; set; }
        public List<TypeIntake> TypeIntake { get; set; }
        public List<Diameter> Diameter { get; set; }
        public List<TypeClient> TypeClients { get; set; }
        public List<TypeAddress> TypeAddresses { get; set; }
        public List<TypeContact> TypeContacts { get; set; }
        public List<ServiceVM> Services { get; set; }
        public List<TypeAgreemnet> TypeAgreemnets { get; set; }
        public List<TypeDiscount> TypeDescounts { get; set; }
    }

    public class TypeClient
    {
        public string IdType { get; set; }
        public string Description { get; set; }
    }

    public class TypeAddress
    {
        public string IdType { get; set; }
        public string Description { get; set; }
    }

    public class TypeContact
    {
        public string IdType { get; set; }
        public string Description { get; set; }
    }

    public class TypeAgreemnet
    {
        public string IdType { get; set; }
        public string Description { get; set; }
    }

    public class TypeDiscount
    {
        public int IdType { get; set; }
        public string Description { get; set; }
        public Int32 Percentage { get; set; }
    }
}
