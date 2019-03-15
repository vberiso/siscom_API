using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TransitPoliceVM
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string UserName { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(150)]
        public string LastName { get; set; }
        [StringLength(150)]
        public string SecondLastName { get; set; }
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [StringLength(150)]
        public string EMail { get; set; }
        [StringLength(300)]
        public string Address { get; set; }
        [StringLength(50)]
        public string Plate { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public int DivitionId { get; set; }

        public string UserId { get; set; }
    }
}
