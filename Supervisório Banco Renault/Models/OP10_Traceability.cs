using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class OP10_Traceability
    {
        public Guid Id { get; set; }
        public string RadiatorCode { get; set; }
        public string CondenserCode { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public bool OP20_Executed { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
