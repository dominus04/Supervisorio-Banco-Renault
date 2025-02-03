using Supervisório_Banco_Renault.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
