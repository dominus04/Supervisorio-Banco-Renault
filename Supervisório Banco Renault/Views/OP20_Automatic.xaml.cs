using Supervisório_Banco_Renault.ViewModels;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para Automatic.xam
    /// </summary>
    public partial class OP20_Automatic : UserControl
    {
        public OP20_Automatic()
        {
            InitializeComponent();
        }

        private void OnPageLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            OP20_AutomaticVM vm = (OP20_AutomaticVM)this.DataContext;
            vm.Start();
        }

        private void OnPageUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            OP20_AutomaticVM vm = (OP20_AutomaticVM)this.DataContext;
            vm.Stop();
        }
    }
}
