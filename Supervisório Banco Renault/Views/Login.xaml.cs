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

        private void TextBoxOnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoginPasswordBox.Focus();
            }
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            LoginVM vm = (LoginVM)DataContext;

            vm.Login(this, LoginPasswordBox.Password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RFIDInputTextBox.Focus();
        }
    }
}
