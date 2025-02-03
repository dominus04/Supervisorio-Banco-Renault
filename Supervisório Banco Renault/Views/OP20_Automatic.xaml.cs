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

        private OP20_AutomaticVM ViewModel()
        {
            return (OP20_AutomaticVM)DataContext;
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

        private void ResetScrapCageMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = ViewModel();
            vm.ResetScrapCage();
        }

        private void ResetProductsCount(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = ViewModel();
            vm.ResetProductsCount();
        }
    }
}
