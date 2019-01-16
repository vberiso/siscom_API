using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Agreement_File")]
    public class AgreementFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_agreement_file")]
        public int Id { get; set; }
        [Column("name"), StringLength(100), Required]
        public string Name { get; set; }
        [Column("extension"), StringLength(4), Required]
        public string extension { get; set; }
        [Column("type"), StringLength(5), Required]
        public string Type { get; set; }
        [Required, Column("is_active")]
        public bool IsActive { get; set; }
        [Column("upload_date"),  Required]
        public DateTime UploadDate { get; set; }
        [Column("description"), StringLength(250)]
        public string Description { get; set; }
        [Column("size"), StringLength(12), Required]
        public string Size { get; set; }

        public int AgreementId { get; set; }
        [JsonIgnore]
        public Agreement Agreement { get; set; }

        public string UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }
    }
}
