using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
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
using Path = System.IO.Path;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para UsersManager.xam
    /// </summary>
    public partial class UsersManager : UserControl
    {

        public UsersManager()
        {
            InitializeComponent();
        }

        private void OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UsersManagerVM vm = (UsersManagerVM)DataContext;

            if (MessageBox.Show("Deseja salvar as alterações?", "Salvar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    vm.UpdateUser(UsersDataGrid.SelectedItem);
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
            else
            {
                e.Cancel = true;
                vm.LoadUsers();
            }
        }

        private void DataGridTagButtonClick(object sender, RoutedEventArgs e)
        {
            UsersDataGrid.CurrentCell = new DataGridCellInfo(UsersDataGrid.SelectedItem, UsersDataGrid.Columns[3]);
            UsersDataGrid.BeginEdit();
        }

        private void AddUserMouseDown(object sender, MouseButtonEventArgs e)
        {
            UsersManagerVM vm = (UsersManagerVM)DataContext;
            vm.AddUser();
            UsersDataGrid.Focus();
            UsersDataGrid.SelectedIndex = UsersDataGrid.Items.Count - 1;
            UsersDataGrid.CurrentCell = new DataGridCellInfo(UsersDataGrid.SelectedItem, UsersDataGrid.Columns[0]);
            UsersDataGrid.BeginEdit();
        }

        private void RemoveUserMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(MessageBox.Show("Deseja remover o usuário selecionado?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            UsersManagerVM vm = (UsersManagerVM)DataContext;
            vm.RemoveUser();
        }

        private void OnBegginingEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if(UsersDataGrid.CurrentCell.Column.DisplayIndex == 0)
            {
                Type t = GetParentService.GetParent(this);
                Libraries.Keyboard.start(t);
            }
        }
    }
}
