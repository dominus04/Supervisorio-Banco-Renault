using Supervisório_Banco_Renault.Core;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP10_MainWindowVM : WindowBaseVM
    {

        public RelayCommand CommandTest { get; set; }

        public OP10_MainWindowVM()
        {
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Automático", "OP10_Automatic"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Manutencao, "Manual", "OP10_Manual"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "I/O", "OP10_IoView"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Rastreabilidade", "OP10_Traceability"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Configurações", "Item3"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Usuários", "UsersManager"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.None, "Login", "Login"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Logoff", "Logoff"));

            CommandTest = new RelayCommand(o => { MessageBox.Show(o.ToString()); }, o => true);
        }
    }
}
