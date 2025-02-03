using Supervisório_Banco_Renault.Models.Enums;
using Supervisório_Banco_Renault.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para UserEdit.xam
    /// </summary>
    /// 
    public partial class UserEdit : Window
    {

        public UserEdit()
        {
            InitializeComponent();
            userLevelComboBox.ItemsSource = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().Take(Enum.GetValues(typeof(AccessLevel)).Length - 1).ToList();
        }

        private void OnClickButtonReadTag(object sender, RoutedEventArgs e)
        {
            tagTextBox.Text = "";
            tagTextBox.Focus();
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Todos os dados serão perdidos. Deseja cancelar?", "Cancelar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private async void Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserEditVM vm = (UserEditVM)DataContext;
            if (MessageBox.Show("Deseja salvar?", "Salvar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (await vm.AddOrUpdateUser())
                {
                    Close();
                }
            }
        }

        private void TagRFIDTextOnKeyDown(object sender, KeyEventArgs e)
        {
            // Lost foocus on enter
            if (e.Key == Key.Enter)
            {
                ReadRFIDButton.Focus();
            }
        }
    }
}
