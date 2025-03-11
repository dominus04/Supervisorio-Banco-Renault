using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Supervisório_Banco_Renault.Models
{
    public class OP20_CurrentProduction
    {
        public string? StepText { get; set; }
        public int ErrorState { get; set; } = 0;
        public string? LabelTraceabilityCode { get; set; }
        public string? TraceabilityCode { get; set; }
        public string? RadiatorCode { get; set; }
        public BitmapImage? StepImage { get; set; }
        public bool labelPrinted { get; set; } = false;
        public bool traceabilitySaved { get; set; } = false;
        public DateTime ProductionDateTime { get; set; }
        public OP10_TraceabilityModel? OP10 { get; set; }
    }
}
