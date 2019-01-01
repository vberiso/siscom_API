using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Model
{
    public class TreeviewItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public bool Disabled { get; set; }
        public bool Checked { get; set; }
        public bool Collapsed { get; set; }
        public TreeviewItem children { get; set; }
    }
}
