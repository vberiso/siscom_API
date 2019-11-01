using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.SOSAPAC
{
    public class FileNotifications
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string IdNotification { get; set; }
        public DateTime GenerationDate { get; set; }
        public string FileName { get; set; }
        public byte[] PDFNotifications { get; set; }
        public IEnumerable<NotificationVM> notifications { get; set; }
        public string TypeFile { get; set; }
        public string Folio { get; set; }
        public int TotalRecords { get; set; }
    }
}
