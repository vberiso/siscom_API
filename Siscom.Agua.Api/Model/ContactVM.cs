using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ContactVM
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMovil { get; set; }
        public int ClientId { get; set; }
    }
}
