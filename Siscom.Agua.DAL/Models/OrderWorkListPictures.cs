using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    public class OrderWorkListPictures
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("capture_date")]
        public DateTime CaptureDate { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("size")]
        public long Size { get; set; }
        [Column("weight"), StringLength(10)]
        public string Weight { get; set; }
        [Column("user")]
        public string User { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("file_picture")]
        public byte[] FilePicture { get; set; }

        public int OrderWorkListId { get; set; }
        public OrderWorkList OrderWorkList { get; set; }
    }
}
