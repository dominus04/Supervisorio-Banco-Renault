using Supervisório_Banco_Renault.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para OP20_Manual.xam
    /// </summary>
    public partial class OP20_Manual : UserControl
    {
        public OP20_Manual()
        {
            InitializeComponent();
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            OP20_ManualVM vm = (OP20_ManualVM)DataContext;
            vm.Start();
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            OP20_ManualVM vm = (OP20_ManualVM)DataContext;
            vm.Stop();
        }
    }
}
