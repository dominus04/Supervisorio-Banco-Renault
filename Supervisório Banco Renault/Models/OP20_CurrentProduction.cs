using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class OP20_CurrentProduction
    {
        public string StepText { get; set; }
        public bool Error { get; set; } = false;
        public string VerifyTraceabilityCode { get; set; }
        public string TraceabilityCode { get; set; }
        public string RadiatorCode { get; set; }

    }
}
