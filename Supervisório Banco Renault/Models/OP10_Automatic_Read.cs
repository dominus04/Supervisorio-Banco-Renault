using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class OP10_Automatic_Read
    {
        public ushort Step { get; set; }

        [S7String(S7StringType.S7String, 50)]
        public string RadiatorTag { get; set; }

        [S7String(S7StringType.S7String, 50)]
        public string CondenserTag { get; set; }
    }
}
