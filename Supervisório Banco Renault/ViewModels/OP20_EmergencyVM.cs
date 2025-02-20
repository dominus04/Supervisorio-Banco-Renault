using S7.Net;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    internal class OP20_EmergencyVM : BaseVM
    {
        private readonly PlcConnection _plcConnection;

        private CancellationTokenSource? _cancellationTokenSource;
        
        public Window? CurrentEmergencyWindow { get; set; }

        private ObservableCollection<string> _alarms = [];

        public ObservableCollection<string> Alarms
        {
            get { return _alarms; }
            set
            {
                _alarms = value;
                OnPropertyChanged(nameof(Alarms));
            }
        }

        public OP20_EmergencyVM(PlcConnection plcConnection)
        {
            this._plcConnection = plcConnection;
            _cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => MonitorEmergency(_cancellationTokenSource.Token));
        }

        private async void MonitorEmergency(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {

                byte alarmByte = await _plcConnection.GetAlarmsList();

                ObservableCollection<string> alarms = [];

                if(alarmByte.SelectBit(3))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CurrentEmergencyWindow?.Close();
                    });
                }

                if (!alarmByte.SelectBit(0))
                    alarms.Add("Botão de emergência acionado.");
                if (!alarmByte.SelectBit(1))
                    alarms.Add("Cortina de luz interrompida.");
                if (!alarmByte.SelectBit(2))
                    alarms.Add("Porta traseira aberta.");

                if (_plcConnection.Plc.IsConnected)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Alarms = alarms;
                    });
                }
                await Task.Delay(100, cancellationToken);
            }
        }
    }
}
