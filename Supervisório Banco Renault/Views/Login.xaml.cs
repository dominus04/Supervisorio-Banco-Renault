using Supervisório_Banco_Renault.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para OP10_Login.xam
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginVM? vm = DataContext as LoginVM;
        }
    }
}
