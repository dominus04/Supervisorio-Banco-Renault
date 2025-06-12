using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    internal class PlcMock : IPlcRaw
    {
        public bool IsConnected => true;

        public void Close() { }

        public void Open() { }

        public async Task<byte[]> ReadBytesAsync(DataType dataType, int db, int startByteAdr, int count)
        {
            await Task.Delay(5);
            return Enumerable.Repeat((byte)255, count).ToArray();
        }

        public async Task<T?> ReadClassAsync<T>(int db, int startByteAdr = 0) where T : class, new()
        {
            await Task.Delay(5);
            return new T();
        }

        public async Task WriteAsync(DataType dataType, int db, int startByteAdr, object value, int bitAdr = -1)
        {
            await Task.Delay(5);
            return;
        }

        public async Task WriteBitAsync(DataType dataType, int db, int startByteAdr, int bitAdr, bool value)
        {
            await Task.Delay(5);
            return;
        }
    }
}
