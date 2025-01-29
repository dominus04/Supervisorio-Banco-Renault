using Supervisório_Banco_Renault.Libraries;
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

        public object? _edittedItem = null;

        public UsersManager()
        {
            InitializeComponent();
        }


        private void AddUserMouseDown(object sender, MouseButtonEventArgs e)
        {
            UsersManagerVM vm = (UsersManagerVM)DataContext;
            var t = GetParentService.GetParent(this);
            VirtualKeyboard.start(t);
            vm.AddOrUpdateUser(t, false);
        }

        private void EditUserMouseDown(object sender, MouseButtonEventArgs e)
        {
            UsersManagerVM vm = (UsersManagerVM)DataContext;
            var t = GetParentService.GetParent(this);
            VirtualKeyboard.start(t);
            vm.AddOrUpdateUser(t, true);
        }

        private void RemoveUserMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(MessageBox.Show("Deseja remover o usuário selecionado?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            UsersManagerVM vm = (UsersManagerVM)DataContext;
            vm.RemoveUser();
        }



    }
}
