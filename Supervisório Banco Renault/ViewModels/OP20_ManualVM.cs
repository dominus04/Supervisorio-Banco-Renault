using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_ManualVM(PlcConnection plcConnection) : BaseVM
    {

        #region Properties

        private CancellationTokenSource? _cancellationTokenSource;



        #endregion

        private ObservableCollection<ManualButton> _manualButtons = new()
        {
            new ManualButton(){ Text = "Conjunto inclinado", ByteIndex = 0, BitIndex = 2 },
            new ManualButton(){ Text = "Conjunto linear", ByteIndex = 0, BitIndex = 3 },
            new ManualButton(){ Text = "Vedação do bocal inclinado", ByteIndex = 0, BitIndex = 4 },
            new ManualButton(){ Text = "Vedação do bocal linear", ByteIndex = 0, BitIndex = 5 },
            new ManualButton(){ Text = "Indexador do produto lado 1", ByteIndex = 0, BitIndex = 6 },
            new ManualButton(){ Text = "Indexador do produto lado 2", ByteIndex = 0, BitIndex = 7 },
            new ManualButton(){ Text = "Indexador da mesa", ByteIndex = 1, BitIndex = 0 }
        };

        public ObservableCollection<ManualButton> ManualButtons
        {
            get => _manualButtons;
            set
            {
                _manualButtons = value;
                OnPropertyChanged(nameof(ManualButtons));
            }
        }

        private async Task WritePLCManual(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await plcConnection.WriteToManual(ManualButtons);

                await Task.Delay(100, cancellationToken);
            }
        }

        public async void Start()
        {
            await plcConnection.ActivateOP20Manual();

            _cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Run(() => WritePLCManual(_cancellationTokenSource.Token));
        }

        public async void Stop()
        {
            await plcConnection.DeactivateOP20Manual();
        }

    }
}
