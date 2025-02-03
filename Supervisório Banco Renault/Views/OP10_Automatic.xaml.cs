using Supervisório_Banco_Renault.ViewModels;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para AutomaticOP10.xam
    /// </summary>
    public partial class OP10_Automatic : UserControl
    {

        private OP10_AutomaticVM ViewModel()
        {
            return (OP10_AutomaticVM)DataContext;
        }
        public OP10_Automatic()
        {
            InitializeComponent();
        }

        private void OnPageLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = ViewModel();
            vm.Start();
        }

        private void OnPageUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = ViewModel();
            vm.Stop();
        }

        private void EndCycleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = ViewModel();
            vm.EndCycle();
        }
    }
}
