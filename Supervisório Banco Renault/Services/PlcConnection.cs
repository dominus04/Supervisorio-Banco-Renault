using NLog;
using S7.Net;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Supervisório_Banco_Renault.Services
{
    public class PlcConnection
    {
        internal IPlcRaw Plc { get; private set; }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public PlcConnection(CpuType cpuType, string ipAdress, short rack, short slot)
        {
            logger.Trace("Iniciando conexão com o plc");
            bool mock = true;

            if (mock)
                Plc = new PlcMock();
            else
                Plc = new PlcWrapper(cpuType, ipAdress, rack, slot);

            Connect();
        }

        public void Connect()
        {
            try
            {
                Plc.Open();
                logger.Info("Conexão bem sucedida.");
            }
            catch (Exception e)
            {
                logger.Error(e, "Não foi possível se conectar com o PLC");
                MessageBox.Show("Não foi possível se conectar ao PLC feche o sistema, regularize a situação do dispositivo e abra novamente.");
                System.Environment.Exit(1);
            }
        }

        public void Disconnect()
        {
            Plc.Close();
            logger.Info("PLC desconectado.");
        }

        public bool IsConnected()
        {
            return Plc.IsConnected;
        }

        private async Task SetPlcBitAsync(S7.Net.DataType dataType, int db, int startByteAdr, int bitAdr, bool value, [CallerMemberName] string? caller = null)
        {
            try
            {
                if (!Plc.IsConnected)
                    throw new Exception("Plc is not connected.");
                await Plc.WriteBitAsync(dataType, db, startByteAdr, bitAdr, value);
                //logger.Trace($"[{caller}] Bit escrito: DB{db}, Byte{startByteAdr}, BitOffset{bitAdr}, Valor: {value}");
            }
            catch (Exception e) 
            {
                logger.Error(e, $"[{caller}] Erro ao escrever bit: DB{db}, Byte{startByteAdr}, BitOffset{bitAdr}, Valor: {value}");
                throw;
            }
        }

        #region OP20

        internal async Task<bool> WriteOP20Recipe(Recipe recipe)
        {
            try 
            { 
                await ActivateOP20Automatic();
                await SetPlcBitAsync(DataType.DataBlock, 8, 0, 2, recipe.VerifyRadiatorLabel);
                await SetPlcBitAsync(DataType.DataBlock, 8, 0, 3, recipe.VerifyTraceabilityLabel);
                await SetPlcBitAsync(DataType.DataBlock, 8, 0, 4, recipe.VerifyCondenserCovers);
                await SetPlcBitAsync(DataType.DataBlock, 8, 0, 5, recipe.VerifyRadiator);
                await SetPlcBitAsync(DataType.DataBlock, 8, 22, 0, recipe.ReadRadiatorLabel);
                await SetPlcBitAsync(DataType.DataBlock, 8, 0, 6, recipe.VerifyCondenser);
                ushort radiatorProgramTemp = Conversion.ConvertToUshort(recipe.AteqRadiatorProgram);
                ushort condenserProgramTemp = Conversion.ConvertToUshort(recipe.AteqCondenserProgram);
                await Plc.WriteAsync(DataType.DataBlock, 8, 2, --radiatorProgramTemp);
                await Plc.WriteAsync(DataType.DataBlock, 8, 4, --condenserProgramTemp);
                await Plc.WriteAsync(DataType.DataBlock, 8, 6, (float)recipe.RadiatorPSMinimum);
                await Plc.WriteAsync(DataType.DataBlock, 8, 10, (float)recipe.RadiatorPSMaximum);
                await Plc.WriteAsync(DataType.DataBlock, 8, 14, (float)recipe.CondenserPSMinimum);
                await Plc.WriteAsync(DataType.DataBlock, 8, 18, (float)recipe.CondenserPSMaximum);
                logger.Info($"Produto {recipe.ModuleCode} selecionado com sucesso na OP20.");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Não foi possível enviar a receita selecionada ao PLC, favor selecionar novamente.", "Erro na seleçao de receitas", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        internal async Task EnableScrapCage()
        {
            await SetPlcBitAsync(DataType.DataBlock, 8, 22, 1, true);
        }

        internal async Task DisableScrapCage()
        {
            await SetPlcBitAsync(DataType.DataBlock, 8, 22, 1, false);
        }

        internal async Task<bool> ReadScrapCageStatus()
        {
            byte alarmByte = (await Plc.ReadBytesAsync(DataType.DataBlock, 8, 22, 1))[0];

            return alarmByte.SelectBit(1);
        }

        #region Automatic

        internal async Task ActivateOP20Automatic()
        {
            await SetPlcBitAsync(DataType.DataBlock, 8, 0, 1, false);
            await SetPlcBitAsync(DataType.DataBlock, 8, 0, 0, true);
        }

        internal async Task DeactivateOP20Automatic()
        {
            await SetPlcBitAsync(DataType.DataBlock, 8, 0, 0, false);
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
            await SetPlcBitAsync(DataType.DataBlock, 8, 0, 0, false);
            await SetPlcBitAsync(DataType.DataBlock, 8, 0, 1, true);
        }

        internal async Task DeactivateOP20Manual()
        {
            await SetPlcBitAsync(DataType.DataBlock, 8, 0, 1, false);
        }

        internal async Task WriteToManual(ObservableCollection<ManualButton> manualButtons)
        {
            ManualButton ?tempButton = null;
            try
            {
                foreach (var button in manualButtons)
                {
                    tempButton = button;
                    await Plc.WriteBitAsync(DataType.DataBlock, 5, button.ByteIndex, button.BitIndex, button.IsChecked);
                }
            }
            catch (Exception ex) 
            {
                logger.Error(ex, $"[WriteToManual] Erro ao escrever bit: DB5, Byte{tempButton?.ByteIndex}, BitOffset{tempButton?.BitIndex}, Valor: {tempButton?.IsChecked}");
            }
        }

        #endregion

        #region Label Control

        internal async Task SetL1RadiatorLabelOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 101, 0, 0, true);
        }

        internal async Task SetL1RadiatorLabelNOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 101, 0, 1, true);
        }

        internal async Task SetL2RadiatorLabelOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 103, 0, 0, true);
        }

        internal async Task SetL2RadiatorLabelNOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 103, 0, 1, true);
        }

        internal async Task SetL1TraceabilityLabelOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 101, 0, 2, true);
        }

        internal async Task SetL1TraceabilityLabelNOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 101, 0, 3, true);
        }

        internal async Task SetL2TraceabilityLabelOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 103, 0, 2, true);
        }

        internal async Task SetL2TraceabilityLabelNOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 103, 0, 3, true);
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
            await SetPlcBitAsync(DataType.DataBlock, 8, 22, 2, true);
        }

        internal async Task ResetOP20ProductsCount()
        {
            await SetPlcBitAsync(DataType.DataBlock, 5, 0, 1, true);
        }

        public async Task ResetScrapCage()
        {
            await SetPlcBitAsync(DataType.DataBlock, 5, 0, 0, true);
        }

        #endregion

        public async Task<OP10_Automatic_Read?> ReadOP10Automatic()
        {
            return await Plc.ReadClassAsync<OP10_Automatic_Read>(110);
        }

        public async Task ActivateOP10Automatic()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 1, false);
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 0, true);
        }

        public async Task WriteOP10Recipe(Recipe recipe)
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 6, recipe.ReadRadiatorLabelOP10);
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 7, recipe.ReadCondenserLabelOP10);
        }

        public async Task DeactivateOP10Automatic()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 0, false);
        }

        public async Task ActivateOP10Manual()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 0, false);
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 1, true);
        }

        public async Task DeactivateOP10Manual()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 1, false);
        }

        public async Task EndCycle()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 2, true);
        }

        public async Task SetOP10DataSaved()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 3, true);
        }

        public async Task SetOP10DataProblem()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 1, 0, true);
        }

        public async Task SetOP10RadiatorLabelOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 4, true);
        }

        public async Task SetOP10RadiatorLabelNOK()
        {
            await SetPlcBitAsync(DataType.DataBlock, 111, 0, 5, true);
        }

    }
}
