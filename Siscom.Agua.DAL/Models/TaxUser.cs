using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{

    [Table("Tax_User")]
    public class TaxUser
    {
        public TaxUser()
        {
            TaxAddresses = new HashSet<TaxAddress>();
            Breaches = new HashSet<Breach>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_tax_user")]
        public int Id { get; set; }
        [Column("name"), Required, StringLength(200)]
        public String Name { get; set; }
        [Column("rfc"), StringLength(17)]
        public String RFC { get; set; }
        [Column("curp"), StringLength(18)]
        public String CURP { get; set; }
        [Column("phone_number"), StringLength(50), Required]
        public String PhoneNumber { get; set; }
        [Column("email"), StringLength(150)]
        public String EMail { get; set; }
        [Column("is_active"), Required]
        public bool IsActive { get; set; }

        public ICollection<TaxAddress>  TaxAddresses { get; set; }
        public ICollection<Breach> Breaches { get; set; }

        public static implicit operator int(TaxUser v)
        {
            throw new NotImplementedException();
        }
    }
}
