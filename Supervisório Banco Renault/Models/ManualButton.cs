using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class ManualButton
    {
        public string Text { get; set; }
        public bool IsChecked { get; set; }
        public int ByteIndex { get; set; }
        public int BitIndex { get; set; }
    }
}
