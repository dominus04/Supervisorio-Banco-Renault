using BCrypt.Net;
using Supervisório_Banco_Renault.Libraries;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
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
    /// Interação lógica para UserEdit.xam
    /// </summary>
    public partial class UserEdit : Window
    {

        public UserEdit()
        {
            InitializeComponent();
        }

        private void OnClickButtonReadTag(object sender, RoutedEventArgs e)
        {
            tagTextBox.Focus();
        }

        private void OnPassChanged(object sender, RoutedEventArgs e)
        {
            UserEditVM vm = (UserEditVM)DataContext;
            vm.User.HashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordBox.Password);
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Todos os dados serão perdidos. Deseja cancelar?", "Cancelar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                UserEditVM vm = (UserEditVM)DataContext;
                this.Close();
            }
        }

        private void Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserEditVM vm = (UserEditVM)DataContext;
            if (MessageBox.Show("Deseja salvar?", "Salvar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                vm.AddOrUpdateUser();
                this.Close();
            }
        }
    }
}
