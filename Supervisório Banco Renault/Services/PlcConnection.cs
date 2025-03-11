using S7.Net;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows;

namespace Supervisório_Banco_Renault.Services
{
    public class PlcConnection
    {
        public Plc Plc { get; private set; }

        public PlcConnection(CpuType cpuType, string ipAdress, short rack, short slot)
        {
            Plc = new Plc(cpuType, ipAdress, rack, slot);
            Connect();
        }

        public void Connect()
        {
            try
            {
                Plc.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Não foi possível se conectar ao PLC feche o sistema, regularize a situação do dispositivo e abra novamente.");
            }
        }

        public void Disconnect()
        {
            Plc.Close();
        }

        public bool IsConnected()
        {
            return Plc.IsConnected;
        }

        #region OP20

        internal async Task<bool> WriteOP20Recipe(Recipe recipe)
        {
            await ActivateOP20Automatic();
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 2, recipe.VerifyRadiatorLabel);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 3, recipe.VerifyTraceabilityLabel);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 4, recipe.VerifyCondenserCovers);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 5, recipe.VerifyRadiator);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 22, 0, recipe.ReadRadiatorLabel);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 6, recipe.VerifyCondenser);
            ushort radiatorProgramTemp = Conversion.ConvertToUshort(recipe.AteqRadiatorProgram);
            ushort condenserProgramTemp = Conversion.ConvertToUshort(recipe.AteqCondenserProgram);
            await Plc.WriteAsync(DataType.DataBlock, 8, 2, --radiatorProgramTemp);
            await Plc.WriteAsync(DataType.DataBlock, 8, 4, --condenserProgramTemp);
            await Plc.WriteAsync(DataType.DataBlock, 8, 6, (float)recipe.RadiatorPSMinimum);
            await Plc.WriteAsync(DataType.DataBlock, 8, 10, (float)recipe.RadiatorPSMaximum);
            await Plc.WriteAsync(DataType.DataBlock, 8, 14, (float)recipe.CondenserPSMinimum);
            await Plc.WriteAsync(DataType.DataBlock, 8, 18, (float)recipe.CondenserPSMaximum);
            return true;
        }

        internal async Task EnableScrapCage()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 22, 1, true);
        }

        internal async Task DisableScrapCage()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 22, 1, false);
        }

        internal async Task<bool> ReadScrapCageStatus()
        {
            byte alarmByte = (await Plc.ReadBytesAsync(DataType.DataBlock, 8, 22, 1))[0];

            return alarmByte.SelectBit(1);
        }

        #region Automatic

        internal async Task ActivateOP20Automatic()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 1, false);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 0, true);
        }

        internal async Task DeactivateOP20Automatic()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 0, false);
        }

        internal async Task<OP20_Automatic_Read?> ReadOP20AutomaticL1()
        {
            var teste = await Plc.ReadClassAsync<OP20_Automatic_Read>(100);
            return teste;
        }

        internal async Task<OP20_Automatic_Read?> ReadOP20AutomaticL2()
        {
            return await Plc.ReadClassAsync<OP20_Automatic_Read>(102);
        }

        internal async Task<OP20_AutomaticCommomR?> ReadOP20AutomaticCommon()
        {
            return await Plc.ReadClassAsync<OP20_AutomaticCommomR>(10);
        }

        #endregion

        #region Manual

        internal async Task ActivateOP20Manual()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 0, false);
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 1, true);
        }

        internal async Task DeactivateOP20Manual()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 0, 1, false);
        }

        internal async Task WriteToManual(ObservableCollection<ManualButton> manualButtons)
        {
            foreach (var button in manualButtons)
            {
                await Plc.WriteBitAsync(DataType.DataBlock, 5, button.ByteIndex, button.BitIndex, button.IsChecked);
            }
        }

        #endregion

        #region Label Control

        internal async Task SetL1RadiatorLabelOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 0, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 0, false);
        }

        internal async Task SetL1RadiatorLabelNOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 1, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 1, false);
        }

        internal async Task SetL2RadiatorLabelOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 0, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 0, false);
        }

        internal async Task SetL2RadiatorLabelNOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 1, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 1, false);
        }

        internal async Task SetL1TraceabilityLabelOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 2, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 2, false);
        }

        internal async Task SetL1TraceabilityLabelNOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 3, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 101, 0, 3, false);
        }

        internal async Task SetL2TraceabilityLabelOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 2, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 2, false);
        }

        internal async Task SetL2TraceabilityLabelNOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 3, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 103, 0, 3, false);
        }

        #endregion


        internal async Task<byte> GetAlarmsList()
        {
            return (await Plc.ReadBytesAsync(DataType.DataBlock, 1, 10, 1))[0];

        }   

        internal async Task<bool> IsPLCEmergencyOK()
        {
            byte alarmByte = (await Plc.ReadBytesAsync(DataType.DataBlock, 1, 10, 1))[0];

            return alarmByte.SelectBit(3);

        }

        internal async Task SetLabelPrinted()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 8, 22, 2, true);
        }

        internal async Task ResetOP20ProductsCount()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 5, 0, 1, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 5, 0, 1, false);
        }

        public async Task ResetScrapCage()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 5, 0, 0, true);
        }

        #endregion

        public async Task<OP10_Automatic_Read?> ReadOP10Automatic()
        {
            return await Plc.ReadClassAsync<OP10_Automatic_Read>(110);
        }

        public async Task ActivateOP10Automatic()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 1, false);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 0, true);
        }

        public async Task WriteOP10Recipe(Recipe recipe)
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 6, recipe.ReadRadiatorLabelOP10);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 7, recipe.ReadCondenserLabelOP10);
        }

        public async Task DeactivateOP10Automatic()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 0, false);
        }

        public async Task ActivateOP10Manual()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 0, false);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 1, true);
        }

        public async Task DeactivateOP10Manual()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 1, false);
        }

        public async Task EndCycle()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 2, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 2, false);
        }

        public async Task SetOP10DataSaved()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 3, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 3, false);
        }

        public async Task SetOP10RadiatorLabelOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 4, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 4, false);
        }

        public async Task SetOP10RadiatorLabelNOK()
        {
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 5, true);
            await Plc.WriteBitAsync(DataType.DataBlock, 111, 0, 5, false);
        }

    }
}
