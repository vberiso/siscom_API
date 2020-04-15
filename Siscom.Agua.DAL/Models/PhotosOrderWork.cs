using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class PhotosOrderWork
    {
        [Column("id_photos_orderWork"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
   
        [Column("path_file")]
        public string PathFile { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("date_photo")]
        public DateTime DatePhoto { get; set; }

        [Column("name_file")]
        public string NameFile { get; set; }

        [Column("size")]
        public long Size { get; set; }

        [Column("user")]
        public string User { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("blob_photo")]
        public byte[] BlobPhoto { get; set; }

        [Column("OrderWorkId")]
        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }

    }
}
