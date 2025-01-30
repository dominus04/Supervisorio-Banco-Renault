using S7.Net;
using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.Services
{
    public class PlcConnection
    {
        public Plc Plc { get; private set; }

        public PlcConnection(CpuType cpuType, string ipAdress, short rack, short slot)
        {
            Plc = new Plc(cpuType, ipAdress, rack, slot);
            try
            {
                //Connect();
            }catch(Exception e)
            {
                MessageBox.Show("Não foi possível se conectar ao PLC feche o sistema, regularize a situação do dispositivo e abra novamente.");
            }
        }

        public void Connect()
        {
            Plc.Open();
        }

        public void Disconnect()
        {
            Plc.Close();
        }

        public bool IsConnected()
        {
            return Plc.IsConnected;
        }

        public async void WriteOP20Recipe(Recipe recipe)
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 0, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 1, recipe.VerifyModuleTag);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 2, recipe.VerifyTraceabilityTag);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 3, recipe.VerifyCondenserCovers);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 4, recipe.VerifyRadiator);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 5, recipe.VerifyCondenser);
            await Plc.WriteAsync(DataType.DataBlock, 8, 2, Conversion.ConvertToUshort(--recipe.AteqRadiatorProgram));
            await Plc.WriteAsync(DataType.DataBlock, 8, 4, Conversion.ConvertToUshort(--recipe.AteqCondenserProgram));
            await Plc.WriteAsync(DataType.DataBlock, 8, 6, (float)recipe.RadiatorPSMinimum);
            await Plc.WriteAsync(DataType.DataBlock, 8, 10, (float)recipe.RadiatorPSMaximum);
            await Plc.WriteAsync(DataType.DataBlock, 8, 14, (float)recipe.CondenserPSMinimum);
            await Plc.WriteAsync(DataType.DataBlock, 8, 18, (float)recipe.CondenserPSMaximum);
        }




    }
}
