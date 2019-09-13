﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model.SOSAPAC
{
    public class FileNotifications
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime GenerationDate { get; set; }
        public string FileName { get; set; }
        public IEnumerable<NotificationVM> notifications { get; set; }
    }
}