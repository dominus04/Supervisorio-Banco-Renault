using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class Label
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LabelBaseLayout { get; set; }
        public string JulianDateFormat { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public int SequentialFormat { get; set; }
        public TimeSpan Tunr1Init { get; set; }
        public TimeSpan Tunr1End { get; set; }
        public TimeSpan Tunr2Init { get; set; }
        public TimeSpan Tunr2End { get; set; }
        public TimeSpan Tunr3Init { get; set; }
        public TimeSpan Tunr3End { get; set; }
    }
}
