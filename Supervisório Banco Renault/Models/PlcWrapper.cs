using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    internal class PlcWrapper : IPlcRaw
    {

        private readonly Plc _plc;

        public PlcWrapper(CpuType cpu, string ip, Int16 rack, Int16 slot)
        {
            _plc = new Plc(cpu, ip, rack, slot);
        }

        public bool IsConnected => _plc.IsConnected;

        public void Close() => _plc.Close();
        
        public void Open() => _plc.Open();

        public async Task<byte[]> ReadBytesAsync(DataType dataType, int db, int startByteAdr, int count) => await _plc.ReadBytesAsync(dataType, db, startByteAdr, count);

        public async Task<T?> ReadClassAsync<T>(int db, int startByteAdr = 0) where T : class, new() => await _plc.ReadClassAsync<T>(db, startByteAdr);

        public async Task WriteAsync(DataType dataType, int db, int startByteAdr, object value, int bitAdr = -1) => await _plc.WriteAsync(dataType, db, startByteAdr, value, bitAdr);

        public async Task WriteBitAsync(DataType dataType, int db, int startByteAdr, int bitAdr, bool value) => await _plc.WriteBitAsync(dataType, db, startByteAdr, bitAdr, value);
    }
}
