using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TypesAgreementVM
    {
        public List<TypeClassificationsVM> TypeClassifications { get; set; }
        public List<TypeIntakeVM> TypeIntake { get; set; }
        public List<TypeServiceVM> TypeService { get; set; }
        public List<TypeUseVM> TypeUse { get; set; }
    }
}
