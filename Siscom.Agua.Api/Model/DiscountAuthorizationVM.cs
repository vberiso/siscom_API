using Siscom.Agua.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class DiscountAuthorizationVM
    {
        public int Id { get; set; }
        [Required]
        public DateTime RequestDate { get; set; }
        public DateTime AuthorizationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal AmountDiscount { get; set; }
        public Int16 DiscountPercentage { get; set; }
        [Required, StringLength(50)]
        public string Account { get; set; }
        [Required, StringLength(50)]
        public string AccountAdjusted { get; set; }
        [Required, StringLength(30)]
        public string Folio { get; set; }
        [StringLength(50)]
        public string KeyFirebase { get; set; }
        public int IdOrigin { get; set; }
        [Required, StringLength(5)]
        public string Type { get; set; }
        [StringLength(5), Required]
        public string Status { get; set; }
        public string Observation { get; set; }
        public string ObservationResponse { get; set; }
        [StringLength(30), Required]
        public string BranchOffice { get; set; }
        public string UserAuthorizationId { get; set; }
        [Required]
        public string UserRequestId { get; set; }
        public string FileNameDB { get; set; }
        [StringLength(200), Required]
        public string FileName { get; set; }
        public string NameUserResponse { get; set; }
        public string NameUserRequest { get; set; }
        public bool IsApplied { get; set; }
        public ApplicationUser UserRequest { get; set; }

        public ICollection<DiscountAuthorizationDetail> DiscountAuthorizationDetails { get; set; }
    }
}
