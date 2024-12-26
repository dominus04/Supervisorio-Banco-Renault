using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Services
{
    public class ReadRFIDService
    {
        public async Task<string> ReadRFID()
        {
            await Task.Delay(1000);
            return "RFID";
        }
    }
}
