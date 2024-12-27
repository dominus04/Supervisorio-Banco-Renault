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
            RFIDInputTextBox.Focus();
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            RFIDInputTextBox.Focus();
        }

        private void TextBoxOnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var vm = DataContext as LoginVM;

                vm.Login(this);

                RFIDInputTextBox.Clear();
            }
        }
    }
}
