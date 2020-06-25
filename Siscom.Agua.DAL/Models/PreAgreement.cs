using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("pre_agreement")]
    public class PreAgreement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_pre_agreement")]
        public int Id { get; set; }

        [Column("folio"), StringLength(30)]
        public string Folio { get; set; }
        
        public int TypeIntakeId { get; set; }
        public TypeIntake TypeIntake { get; set; }

        public int TypeServiceId { get; set; }
        public TypeService TypeService { get; set; }

        public int TypeUseId { get; set; }
        public TypeUse TypeUse { get; set; }

        public int TypeClassificationId { get; set; }
        public TypeClassification TypeClassification { get; set; }

        [Column("client_name"), StringLength(200)]
        public string ClientName { get; set; }
        [Column("client_last_name"), StringLength(80)]
        public string ClientLastName { get; set; }
        [Column("client_second_last_name"), StringLength(80)]
        public string ClientSecondLastName { get; set; }

        [Column("street"), StringLength(150)]
        public string Street { get; set; }
        [Column("outdoor"), StringLength(50)]
        public string Outdoor { get; set; }
        [Column("indoor"), StringLength(50)]
        public string Indoor { get; set; }
        [Column("zip"), StringLength(5)]
        public string Zip { get; set; }
        [Column("reference"), StringLength(200)]
        public string Reference { get; set; }
        [Column("lat"), StringLength(20)]
        public string Lat { get; set; }
        [Column("Lon"), StringLength(20)]
        public string Lon { get; set; }

        public int SuburbsId { get; set; }
        public Suburb Suburbs { get; set; }

        [Column("service_id_1")]
        public int ServiceId1 { get; set; }
        [Column("service_id_2")]
        public int ServiceId2 { get; set; }
        [Column("service_id_3")]
        public int ServiceId3 { get; set; }

        [Column("registration_reason")]
        public string RegistrationReason { get; set; }
        [Column("observation")]
        public string Observation { get; set; }
        [Column("status"), StringLength(6)]
        public string Status { get; set; }
        [Column("date_registration"), Required]
        public DateTime DateRegistration { get; set; }
                
        public int OrderWorkId { get; set; }
        public string folio_order_result { get; set; }
        public int agreementId_new { get; set; }
    }
}
