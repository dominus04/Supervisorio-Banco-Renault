using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    internal interface IPlcRaw
    {
        public bool IsConnected { get; }

        void Open();
        void Close();
        public Task WriteBitAsync(DataType dataType, int db, int startByteAdr, int bitAdr, bool value);
        public Task<byte[]> ReadBytesAsync(DataType dataType, int db, int startByteAdr, int count);
        public Task<T?> ReadClassAsync<T>(int db, int startByteAdr = 0) where T : class, new();
        public Task WriteAsync(DataType dataType, int db, int startByteAdr, object value, int bitAdr = -1);
    }
}
