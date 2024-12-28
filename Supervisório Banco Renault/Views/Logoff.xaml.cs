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
    /// Interação lógica para Logoff.xam
    /// </summary>
    public partial class Logoff : UserControl
    {
        public Logoff()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Deseja realmente sair?", "Sair", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                (DataContext as LogoffVM)?.Logoff(this);
        }
    }
}
