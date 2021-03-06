using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class ClientVM
    {
        public int Id { get; set; }
        [StringLength(200), Required]
        public string Name { get; set; }
        [StringLength(80), Required]
        public string LastName { get; set; }
        [StringLength(80), Required]
        public string SecondLastName { get; set; }
        [StringLength(13)]
        public string RFC { get; set; }
        [StringLength(18)]
        public string CURP { get; set; }
        [StringLength(13)]
        public string INE { get; set; }
        [StringLength(150)]
        public string EMail { get; set; }
        [StringLength(5)]
        public string TypeUser { get; set; }

        public bool TaxRegime { get; set; }

        public bool IsMale { get; set; }
        public bool IsActive { get; set; }

        public List<ContactVM> Contacts { get; set; }
    }

    public class UpdateClient
    {
        public UpdateClient()
        {
            Client = new List<ClientVM>();
        }
        public List<ClientVM> Client { get; set; }
    }
}
