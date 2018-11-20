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
        public List<TypeCommertialBusiness> TypeCommertialBusiness { get; set; }
        public List<TypeStateService> TypeStateService { get; set; }
        public List<TypeIntake> TypeIntake { get; set; }
        public List<Diameter> Diameter { get; set; }
    }
}
